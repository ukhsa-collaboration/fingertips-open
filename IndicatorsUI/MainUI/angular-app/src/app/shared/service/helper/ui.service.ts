import { Injectable } from '@angular/core';

/** Wrapper for JQuery access to page elements not defined in Angular */
@Injectable()
export class UIService {

  setScrollTop(scrollTop: number): void {
    if (scrollTop) {
      $(window).scrollTop(scrollTop);
    }
  }

  getScrollTop(): number {
    return $(window).scrollTop();
  }

  toggleQuintileLegend($element, useQuintileColouring) {
    if (!useQuintileColouring) {
      $element.hide();
    } else {
      $element.show();
    }
  }
}
