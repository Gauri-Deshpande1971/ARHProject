import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
export interface DropdownOption {
  value: number;
  name: string;
}
@Injectable({
  providedIn: 'root'
})
export class DropdownService {
  baseurl = environment.apiUrl;    
  constructor(private http: HttpClient) {}

  getCategoryOptions(): Observable<DropdownOption[]> {debugger;
    return this.http.get<DropdownOption[]>(this.baseurl+'Category/GetCategoryDropDownList');
  }
  getStreamNames(): Observable<DropdownOption[]> {debugger;
    return this.http.get<DropdownOption[]>(this.baseurl+'Stream/GetStreamList');
  }
   getContenttypenames(): Observable<DropdownOption[]> {debugger;
      return this.http.get<DropdownOption[]>(this.baseurl+'Content_type/GetContentTypesDropDownList');
    }
 }
