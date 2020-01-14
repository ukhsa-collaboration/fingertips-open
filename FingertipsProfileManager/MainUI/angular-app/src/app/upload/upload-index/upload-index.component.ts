import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { UploadService } from '../../services/upload.service';

@Component({
  selector: 'app-upload-index',
  templateUrl: './upload-index.component.html',
  styleUrls: ['./upload-index.component.css']
})
export class UploadIndexComponent implements OnInit {

  // Incoming members
  @Input() batchTemplateUrl: string;
  @Input() batchLastUpdated: string;

  // Outgoing members
  @Output() emitNavigateToYourUploadJobsTab = new EventEmitter();

  // public members
  public actionOption = 'true';
  public fileTypeAllowed = true;
  public fileReadyForUpload = false;
  public fileName = '';
  public fileToUpload: File;

  constructor(private uploadService: UploadService) { }

  ngOnInit() {
  }

  setFileName(event: any): void {
    this.resetFileUpload();

    if (event.target.files.length > 0) {
      this.fileToUpload = event.target.files[0];
      this.fileName = this.fileToUpload.name;
      this.fileReadyForUpload = this.fileTypeAllowed = this.isFileTypeAllowed(this.fileToUpload.type);
    }
  }

  onSelectActionOptionsChange(event: any) {
    this.actionOption = event.target.value;
  }

  isFileTypeAllowed(fileType: string): boolean {
    if (fileType.length > 0) {
      if (fileType === 'application/vnd.ms-excel' || fileType.indexOf('spreadsheetml') > -1) {
        return true;
      }
    }

    return false;
  }

  uploadFile() {
    this.uploadService.uploadFile(this.fileToUpload, this.actionOption).subscribe(
      (response) => {
        this.emitNavigateToYourUploadJobsTab.emit();
      },
      (error) => {
        console.log('Failed to upload file: ' + this.fileToUpload.name);
      }
    );
  }

  cancel() {
    this.resetFileUpload();
  }

  resetFileUpload() {
    this.fileToUpload = undefined;
    this.fileName = '';
    this.fileReadyForUpload = false;
    this.fileTypeAllowed = true;
  }
}
