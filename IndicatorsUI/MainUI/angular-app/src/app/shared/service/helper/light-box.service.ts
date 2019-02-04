import { Injectable } from '@angular/core';
import { LightBoxConfig } from '../../component/light-box/light-box.component';
import { FTHelperService } from './ftHelper.service';

@Injectable()
export class LightBoxService {

  constructor(private ftHelperService: FTHelperService) { }

  /**
   * Display a light box on the Fingertips data page
   */
  display(lightBoxConfig: LightBoxConfig) {

    // To enable a full screen lightbox on the data page
    const html = `<div style="padding:15px;">
    <h3>${lightBoxConfig.Title}</h3>
    <div>
    ${lightBoxConfig.Html}
    </div>
    <div class="lightbox-button-box">
    <button class="btn btn-primary active lightbox-button" onclick="lightbox.hide()">OK</button>
    </div>
    </div>`;
    const popupWidth = 500;
    const left = ($(window).width() - popupWidth) / 2;
    const top = lightBoxConfig.Top;
    this.ftHelperService.lightboxShow(html, top, left, popupWidth);
  }
}

