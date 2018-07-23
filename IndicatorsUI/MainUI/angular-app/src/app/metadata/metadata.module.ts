import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {MetadataComponent} from './metadata.component';
import {MetadataTableComponent} from './metadata-table/metadata-table.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    MetadataComponent,
    MetadataTableComponent
  ],
  exports: [
    MetadataComponent,
    MetadataTableComponent
  ],
})
export class MetadataModule { }
