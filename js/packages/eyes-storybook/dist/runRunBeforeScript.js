
function __runRunBeforeScript(...args) {
  var runRunBeforeScript = (function () {
  'use strict';

  const API_VERSIONS = {
    v4: 'v4',
    v5: 'v5',
    v5_2: 'v5_2',
    v6_4: 'v6_4',
  };

  function getClientAPI() {
    const frameWindow = getFrameWindow();
    const clientAPI = frameWindow.__STORYBOOK_CLIENT_API__;
    const addons = frameWindow.__STORYBOOK_ADDONS;

    return getAPI(getStorybookVersion());

    function getStorybookVersion() {
      const addons = frameWindow.__STORYBOOK_ADDONS;

      if (frameWindow.__STORYBOOK_PREVIEW__) {
        return API_VERSIONS.v6_4;
      } else if (frameWindow.__STORYBOOK_STORY_STORE__) {
        return API_VERSIONS.v5_2;
      } else if (frameWindow.__STORYBOOK_CLIENT_API__ && frameWindow.__STORYBOOK_CLIENT_API__.raw) {
        return API_VERSIONS.v5;
      } else if (
        addons &&
        addons.channel &&
        addons.channel._listeners &&
        addons.channel._listeners.setCurrentStory &&
        addons.channel._listeners.setCurrentStory[0]
      ) {
        return API_VERSIONS.v4;
      } else {
        throw new Error("Cannot get client API: couldn't detect storybook version");
      }
    }
    function onStoryRendered(callback) {
      if (addons && addons.channel && addons.channel.once) {
        addons.channel.once('storyRendered', () => {
          setTimeout(callback, 0);
        });
      } else {
        callback();
      }
    }
    function getAPI(version) {
      if (version) {
        let api;
        switch (version) {
          case API_VERSIONS.v4: {
            api = {
              getStories: () => {
                if (!frameWindow.__APPLITOOLS_STORIES) {
                  frameWindow.__APPLITOOLS_STORIES = Object.values(clientAPI._storyStore._data)
                    .map(({stories, kind}) => Object.values(stories).map(s => ({...s, kind})))
                    .flat();
                }
                return frameWindow.__APPLITOOLS_STORIES;
              },
              selectStory: i => {
                const {kind, name: story} = api.getStories()[i];
                addons.channel._listeners.setCurrentStory[0]({kind, story});
              },
              onStoryRendered,
            };
            break;
          }

          case API_VERSIONS.v5: {
            api = {
              getStories: () => {
                return clientAPI.raw();
              },
              selectStory: i => {
                clientAPI._storyStore.setSelection(clientAPI.raw()[i]);
              },
              onStoryRendered,
            };
            break;
          }

          case API_VERSIONS.v5_2: {
            api = {
              getStories: () => {
                return clientAPI.raw();
              },
              selectStory: i => {
                frameWindow.__STORYBOOK_STORY_STORE__.setSelection({storyId: clientAPI.raw()[i].id});
              },
              onStoryRendered,
            };
            break;
          }

          case API_VERSIONS.v6_4: {
            api = {
              getStories: () => {
                return clientAPI.raw();
              },
              selectStory: async i => {
                frameWindow.__STORYBOOK_PREVIEW__.urlStore.setSelection({
                  storyId: clientAPI.raw()[i].id,
                });
                await frameWindow.__STORYBOOK_PREVIEW__.renderSelection();
              },
              onStoryRendered,
            };
            break;
          }
        }

        return {version, ...api};
      }
    }
  }

  function getFrameWindow() {
    if (/iframe.html/.test(window.location.href)) {
      return window;
    }

    const innerFrameWindow = Array.prototype.find.call(window.frames, frame => {
      try {
        return /\/iframe.html/.test(frame.location.href);
      } catch (e) {}
    });

    if (innerFrameWindow) {
      return innerFrameWindow;
    }

    if (window.__STORYBOOK_CLIENT_API__) {
      return window;
    }

    throw new Error('Cannot get client API: no frameWindow');
  }

  var getClientAPI_1 = getClientAPI;

  function getStoryByIndex(index) {
    let api;
    try {
      api = getClientAPI_1();
      const story = api.getStories()[index];
      if (!story) {
        console.log('error cannot get story', index);
      }
      return story;
    } catch (ex) {
      throw new Error(JSON.stringify({message: ex.message, version: api ? api.version : undefined}));
    }
  }

  var getStoryByIndex_1 = getStoryByIndex;

  function runRunBeforeScript(index) {
    try {
      const story = getStoryByIndex_1(index);
      if (!story) return;
      return story.parameters.eyes.runBefore({rootEl: document.getElementById('root'), story: story});
    } catch (ex) {
      return {message: ex.message};
    }
  }

  var runRunBeforeScript_1 = runRunBeforeScript;

  return runRunBeforeScript_1;

}());

  return runRunBeforeScript.apply(this, args);
}
module.exports = __runRunBeforeScript