from __future__ import absolute_import, unicode_literals

import os
import re
import shutil
from collections import OrderedDict
from enum import Enum
from typing import TYPE_CHECKING, Any, Generator, Optional, Text, Type

import six
import yaml
from robot.libraries.BuiltIn import BuiltIn

from applitools.common import RectangleSize, Region
from applitools.common.fluent.target_path import RegionLocator
from applitools.common.utils import argument_guard
from applitools.common.validators import is_webelement
from applitools.selenium import Target, TargetPath
from applitools.selenium.fluent import SeleniumCheckSettings

from .keywords_list import CHECK_SETTINGS_KEYWORDS_LIST, TARGET_PATH_KEYWORDS_LIST

if TYPE_CHECKING:
    from .base import LibraryComponent

SEPARATOR = object()


def extract_keyword_and_arguments(
    keywords_from_test,  # type: list[Any]|tuple[Any]
    defined_keywords,  # type: list[Any]|tuple[Any]
    skip_keywords=None,  # type: Optional[list[Any]]
):
    # type: (...) -> Generator[tuple[Text, list[Any]], None, None]
    res = OrderedDict()
    key_keyword = None
    key_keyword_index = -1
    for i, keyword in enumerate(keywords_from_test):
        if keyword == key_keyword and key_keyword_index != i:
            # Next keyword with similar name
            res[key_keyword].append(SEPARATOR)

        if keyword in defined_keywords:
            key_keyword = keyword
            key_keyword_index = i
            if key_keyword not in res:
                res[key_keyword] = []
            continue

        if key_keyword is None:
            raise ValueError(
                "Incorrect keyword argument. Keywords: {}".format(keywords_from_test)
            )
        res[key_keyword].append(keyword)
    for keyword, arguments in res.items():
        if skip_keywords and keyword in skip_keywords:
            continue
        yield keyword, arguments


def splits_args_by_separator(args):
    # type: (list[Any]) -> Generator[tuple[Any], None, None]
    res = ()
    for i, arg in enumerate(args, start=1):
        if arg is not SEPARATOR:
            res += (arg,)
        else:
            yield res
            res = ()
            continue

        if len(args) <= i:
            yield res


def collect_target_path(*keywords):
    # type: (tuple[Any])->RegionLocator
    target_path = TargetPath()
    for keyword, keyword_args in extract_keyword_and_arguments(
        keywords,
        defined_keywords=TARGET_PATH_KEYWORDS_LIST + CHECK_SETTINGS_KEYWORDS_LIST,
        skip_keywords=CHECK_SETTINGS_KEYWORDS_LIST,
    ):
        # keyword has arguments
        for separated_args in splits_args_by_separator(keyword_args):
            separated_args += (target_path,)
            target_path = BuiltIn().run_keyword(keyword, *separated_args)
    return target_path


def try_resolve_tag_and_keyword(tag, check_settings_keywords, defined_keywords):
    if tag in defined_keywords:
        check_settings_keywords = (tag,) + check_settings_keywords
        tag = None
    return check_settings_keywords, tag


def collect_check_settings_with_target_path(library_component, *check_keywords):
    # type: (LibraryComponent, tuple[Any])->SeleniumCheckSettings
    defined_keywords = TARGET_PATH_KEYWORDS_LIST + CHECK_SETTINGS_KEYWORDS_LIST
    skip_check_settings_keywords = TARGET_PATH_KEYWORDS_LIST
    target_path = collect_target_path(*check_keywords)
    library_component.set_current_check_settings(Target.region(target_path))

    return _collect_check_settings(
        None,
        library_component.current_check_settings,
        defined_keywords,
        skip_check_settings_keywords,
        *check_keywords
    )


def collect_check_settings_with_tag_and_target_path(
    tag, library_component, *check_keywords
):
    # type: (Optional[str], LibraryComponent, tuple[Any])->SeleniumCheckSettings
    defined_keywords = TARGET_PATH_KEYWORDS_LIST + CHECK_SETTINGS_KEYWORDS_LIST
    skip_check_settings_keywords = TARGET_PATH_KEYWORDS_LIST

    check_keywords, tag = try_resolve_tag_and_keyword(
        tag, check_keywords, defined_keywords
    )

    target_path = collect_target_path(*check_keywords)
    library_component.set_current_check_settings(Target.region(target_path))

    return _collect_check_settings(
        tag,
        library_component.current_check_settings,
        defined_keywords,
        skip_check_settings_keywords,
        *check_keywords
    )


def collect_check_settings_with_tag(tag, check_settings, *check_settings_keywords):
    # type: (Optional[str],SeleniumCheckSettings,tuple[Any])->SeleniumCheckSettings
    """Fill `check_setting` with data from keyword and return `check_settings`"""
    defined_keywords = CHECK_SETTINGS_KEYWORDS_LIST
    skip_keywords = []
    check_settings_keywords, tag = try_resolve_tag_and_keyword(
        tag, check_settings_keywords, defined_keywords
    )

    return _collect_check_settings(
        tag, check_settings, defined_keywords, skip_keywords, *check_settings_keywords
    )


def collect_check_settings(check_settings, *check_settings_keywords):
    # type: (SeleniumCheckSettings,tuple[Any])->SeleniumCheckSettings
    """Fill `check_setting` with data from keyword and return `check_settings`"""
    defined_keywords = CHECK_SETTINGS_KEYWORDS_LIST
    skip_keywords = []

    return _collect_check_settings(
        None, check_settings, defined_keywords, skip_keywords, *check_settings_keywords
    )


def _collect_check_settings(
    tag, check_settings, defined_keywords, skip_keywords, *keywords
):
    # type: (Optional[str],SeleniumCheckSettings,list[str],list[str],tuple[Any])->SeleniumCheckSettings
    """Fill `check_setting` with data from keyword and return `check_settings`"""
    if tag is not None:
        check_settings = check_settings.with_name(tag)

    for keyword, keyword_args in extract_keyword_and_arguments(
        keywords, defined_keywords, skip_keywords=skip_keywords
    ):
        if keyword_args:
            # keyword has arguments
            for separated_args in splits_args_by_separator(keyword_args):
                BuiltIn().run_keyword(keyword, *separated_args)
        else:
            # in case keyword without args
            BuiltIn().run_keyword(keyword)
    return check_settings


int_float_pattern = r"\d+(?:\.\d+)?"
padding_rx = re.compile(r"(left|right|top|bottom):\s?(-?\d+)")
padding_match_rx = re.compile(r"^\s?(" + padding_rx.pattern + r"\s?)+$")


def parse_viewport_size(text):
    # type: (Text) -> RectangleSize
    num_ptrs = (int_float_pattern,) * 2
    match = re.match(r"\[(%s) (%s)\]" % num_ptrs, text)
    if match is None:
        raise ValueError(
            "Incorrect value of viewport: {}.\n\t Format should be: [800 700]".format(
                text
            )
        )
    groups = match.groups()
    return RectangleSize(width=float(groups[0]), height=float(groups[1]))


def parse_region(text):
    num_ptrs = (int_float_pattern,) * 4
    match = re.match(r"\[(%s) (%s) (%s) (%s)\]" % num_ptrs, text)
    if match is None:
        raise ValueError(
            "Incorrect value of region: {}.\n\t Format should be: [10 10 10 10]".format(
                text
            )
        )
    groups = match.groups()
    return Region(
        left=float(groups[0]),
        top=float(groups[1]),
        width=float(groups[2]),
        height=float(groups[3]),
    )


def parse_padding(text):
    if text is None:
        return None
    try:
        return int(text)
    except ValueError:
        return parse_padding_dict(text)


def parse_padding_dict(text):
    if padding_match_rx.match(text):
        return {match[0]: int(match[1]) for match in padding_rx.findall(text)}
    else:
        raise ValueError(
            "Incorrect padding value: {}.\n\t Expected format: left: 1 right: 2".format(
                text
            )
        )


def is_webelement_guard(element):
    argument_guard.is_valid_type(
        is_webelement(element),
        "element argument should be type Selenium or Appium Web Element",
    )


def get_config_file_path():
    # type: () -> Text
    """Return path to config file."""
    here = os.path.abspath(os.path.dirname(__file__))
    return os.path.join(here, "applitools.yaml")


def copy_config_to(path_to_dir):
    # type: (Text) -> None
    """Copy config file to path_to_dir."""
    if not os.path.exists(path_to_dir):
        raise ValueError("Directory doesn't exists")
    shutil.copy(get_config_file_path(), path_to_dir)


def get_enum_by_name(name, enm):
    # type: (Text, Type[Enum]) -> Enum
    """Return enum by name."""
    try:
        return getattr(enm, name)
    except AttributeError:
        raise ValueError("`{}` does not contain `{}`".format(enm, name))


def get_enum_by_upper_name(name, enm):
    # type: (Text, Type[Enum]) -> Enum
    """"""
    return get_enum_by_name(name.upper(), enm)


if six.PY2:

    def unicode_yaml_load(stream):
        """Load yaml file with unicode support. Required for Python 2.7."""

        class UnicodeLoader(yaml.SafeLoader):
            @staticmethod
            def unicode_constructor(loader, node):
                scalar = loader.construct_scalar(node)
                return six.text_type(scalar)

            def __init__(self, *args, **kwargs):
                """Load YAML files with unicode support."""
                super(UnicodeLoader, self).__init__(*args, **kwargs)
                self.add_constructor("tag:yaml.org,2002:str", self.unicode_constructor)

        return yaml.load(stream, Loader=UnicodeLoader)

else:
    unicode_yaml_load = yaml.safe_load
