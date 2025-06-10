import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-download-apk',
  templateUrl: './download-apk.component.html',
  styleUrls: ['./download-apk.component.scss']
})
export class DownloadApkComponent implements OnInit {

  baseurl = environment.apiUrl;

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  DownloadApk() {
    let fileName = 'ehsapp.apk';
    this.http.get(this.baseurl + 'account/downloadapk', { responseType: 'blob' as 'blob' })
    .subscribe((fileData: any) => {
    const blob: any = new Blob([fileData], { type: 'application/vnd.android.package-archive' });
    let link = document.createElement('a');
    if (link.download !== undefined) {
        let url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    this.toastr.success('Downloaded successfully');
    },
    error => {
        this.toastr.error('Error downloading APK');
    });
  }

}
