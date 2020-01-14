import { Component, ElementRef, ViewChild, AfterViewInit } from '@angular/core';

@Component({
  selector: 'ft-data-view',
  templateUrl: './data-view.component.html',
  styleUrls: ['./data-view.component.css']
})

/** Components for viewing data in Fingertips */
export class DataViewComponent implements AfterViewInit {

  private observer: MutationObserver;
  @ViewChild('dataViewContainer', { static: true }) public dataViewContainer: ElementRef;

  // Change the element !important from the display content to fix a bug in IE
  static preventImportantElementFromDisplayTag(elementRef: Element) {
    const regexToFind = /(?=display:).*?(!important)/g;
    const regexToChange = /(?=display:).*?(?= !important)/g;
    const display = elementRef.attributes['style'].value;
    const found = display.match(regexToFind);
    if (found != null) {
      const replacement = display.match(regexToChange);
      let newDisplayMode = display;
      replacement.forEach((element, index) => {
        newDisplayMode = newDisplayMode.replace(found[index], element);
      });

      elementRef.attributes['style'].value = newDisplayMode;
      return true;
    }
    return false;
  }

  constructor() { }

  ngAfterViewInit() {
    const isIE = /(MSIE|Trident)/i.test(navigator.userAgent) || !!document['documentMode'];

    if (isIE) {
      this.observeStyleChangesRemovingImportanInDisplay();
    }
  }

  // Create event listener for changes into the style and removing any !important adding into display
  observeStyleChangesRemovingImportanInDisplay() {
    const elementContainer = this.dataViewContainer.nativeElement;

    this.observer = new MutationObserver(mutations => {
      mutations.forEach(function (mutation) {
        if (mutation.attributeName === 'style') {
          DataViewComponent.preventImportantElementFromDisplayTag(<any>mutation.target);
        }
      });
    });
    const config = { attributes: true, subtree: true, attributefilter: ['style'] };

    this.observer.observe(elementContainer, config);
  }
}
