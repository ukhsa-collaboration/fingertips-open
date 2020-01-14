import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IndicatorDropdownComponent } from './indicator-dropdown.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TypeaheadModule } from 'ngx-bootstrap';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
    imports: [
        CommonModule,
        NgbModule,
        FormsModule,
        TypeaheadModule.forRoot(),
        BrowserAnimationsModule
    ],
    declarations: [
        IndicatorDropdownComponent
    ],
    exports: [
        IndicatorDropdownComponent
    ]
})

export class IndicatorDropdownModule { }