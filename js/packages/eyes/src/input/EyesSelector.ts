export type EyesSelector<TSelector = never> =
  | TSelector
  | string
  | {
      selector: TSelector | string
      type?: string
      shadow?: EyesSelector<TSelector>
      frame?: EyesSelector<TSelector>
    }
