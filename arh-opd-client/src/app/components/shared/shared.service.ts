import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import {environment} from '../../../environments/environment'
import { INavMenu, IOTPPassword, MostDownloadedContent } from './models/models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  baseurl = environment.apiUrl;          

  constructor(private http: HttpClient, private toastr: ToastrService) { }

  //  -----------------
  //  Common Services
  //  -----------------
  
  public dateformatter(params: { value: string | null | undefined; } | null | undefined, dateformat: string | null | undefined) {
    if (params !== undefined && params !== null) {
      if (params.value !== undefined && params.value !== null) {
        //  // console.log('In date formatter - ' + params + ', ' + dateformat + ', ' + parseInt(params.value.substr(6)));

        var nowDate = new Date();
        if (params.value.toString().includes('/Date')) {
          nowDate = new Date(parseInt(params.value.substr(6)));
        }
        else {
          nowDate = new Date(params.value.substr(0, 10));
        }
        
        var strArray = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var d = nowDate.getDate();
        var mm = nowDate.getMonth();
        var m = strArray[mm];
        var y = nowDate.getFullYear();

        if (dateformat !== undefined && dateformat !== null && dateformat === 'dd-MMM-yyyy') {
          return '' + (d <= 9 ? '0' + d : d) + '-' + m + '-' + y;
        }
        return '' + (d <= 9 ? '0' + d : d) + '-' 
                  + (mm < 9 ? '0' + (mm+1) : (mm+1)) 
                  + '-' + y;
      }
    }

    return '';
  }

  public date_ymd(params: any | undefined) {
    if (params !== undefined && params !== null) {
        var nowDate = params;
        
        //var strArray = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        var d = nowDate.getDate();
        var mm = nowDate.getMonth();
        //var m = strArray[mm];
        var y = nowDate.getFullYear();

        return '' + y + '-'
                  + (mm < 9 ? '0' + (mm+1) : (mm+1)) 
                  + '-' + (d <= 9 ? '0' + d : d);
      }
      else {
        // Return a default value when params is undefined or null
        return ''; // or you can return something else like 'Invalid Date'
      }
    }

  public basedateformatter(params: { toString: () => string | string[]; value: string; substr: (arg0: number, arg1: number) => string | number | Date; } | null | undefined, dateformat: string | null | undefined) {
    if (params !== undefined && params !== null) {
      //  // console.log('In date formatter - ' + params + ', ' + dateformat + ', ' + parseInt(params.value.substr(6)));

      var nowDate = new Date();
      if (params.toString().includes('/Date')) {
        nowDate = new Date(parseInt(params.value.substr(6)));
      }
      else {
        nowDate = new Date(params.substr(0, 10));
      }
      
      var strArray = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      var d = nowDate.getDate();
      var mm = nowDate.getMonth();
      var m = strArray[mm];
      var y = nowDate.getFullYear();

      if (dateformat !== undefined && dateformat !== null && dateformat === 'dd-MMM-yyyy') {
        return '' + (d <= 9 ? '0' + d : d) + '-' + m + '-' + y;
      }
      return '' + (d <= 9 ? '0' + d : d) + '-' 
                + (mm < 9 ? '0' + (mm+1) : (mm+1)) 
                + '-' + y;
    }

    return '';
  }

  public dmy_to_ymd(dval: string | null | undefined) {
    if (dval !== undefined && dval !== null && dval.length >= 10) {
      var nval = dval.substr(0, 10).split('-');
      if (nval.length === 3) {
        return nval[2] + '-' + nval[1] + '-' + nval[0];
      }
    }

    return dval;
  }

  public ymd_to_dmy(dval: string | null | undefined) {
    if (dval !== undefined && dval !== null && dval.length >= 10) {
      var nval = dval.substr(0, 10).split('-');
      if (nval.length === 3) {
        return nval[2] + '-' + nval[1] + '-' + nval[0];
      }
    }

    return dval;
  }

  public get_hh_mm(params: string | number | Date | null | undefined) {
    if (params !== undefined && params !== null) {
      //  // console.log('In date formatter - ' + params + ', ' + dateformat + ', ' + parseInt(params.value.substr(6)));

      var nowDate = new Date(params);
      
      var strArray = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      var d = nowDate.getDate();
      var mm = nowDate.getMonth();
      var m = strArray[mm];
      var y = nowDate.getFullYear();
      var hh = nowDate.getHours();
      var mins = nowDate.getMinutes();

      return '' + (hh <= 9 ? '0' + hh : hh) + ':' 
                + (mins < 9 ? '0' + (mins) : (mins));
    }

    return '';
  }

  public get_hh_mm_from_dmy(params: string | null | undefined) {
    if (params !== undefined && params !== null && params.length >= 16) {
      return params.substr(11,5);
    }

    return '';
  }
  public GetTraffic(param:any)
  {
    return this.http.get(this.baseurl+ 'Member/Traffic?type='+param);
  }
  public GetStreamList(){
    return this.http.get(this.baseurl+ 'Stream/GetAllStreams');
  }
  public GetClassList(){debugger;
    return this.http.get(this.baseurl+ 'Classes/GetAllClasses');
    }
    public GetSubjectList(){
      return this.http.get(this.baseurl+ 'Subject/GetAllSubjects');
    }
  public GetBooksList(){debugger;
    return this.http.get(this.baseurl+ 'Book/GetallBooks');
    }
    public GetBookById(bookId:any){
        return this.http.get(this.baseurl+ 'Book/GetBookById?id='+bookId);
    }

  public GetContentsList(){debugger;
      return this.http.get(this.baseurl+ 'Content/GetAllContents');
      }
  public GetCategoryList(){
    return this.http.get(this.baseurl+ 'Category/GetAllCategories');
  }
  public GetContentTypesList()
  {
     return this.http.get(this.baseurl+ 'Content_type/GetAllContentTypes');
  }
  public GetLoginCaptcha() {
    localStorage.removeItem('token');
    return this.http.get(this.baseurl + 'account/getcaptchaimage');
  }

  public GetRegisterCaptcha() {
    localStorage.removeItem('token');
    return this.http.get(this.baseurl + 'account/getregistercaptchaimage');
  }

  public GetResetCaptcha() {
    localStorage.removeItem('token');
    return this.http.get(this.baseurl + 'account/getresetcaptchaimage');
  }

  public ChangePassword(data: any) {
    return this.http.post(this.baseurl + 'account/changepassword',data);
  }

  public GetUserProfile() {
    return this.http.get(this.baseurl + 'account/myprofile');
  }

  public ResetFailedCount(usertype: any, username: any) {
    let rfc = {
      userType: usertype,
      userName: username
    }

    return this.http.post(this.baseurl + 'account/resetfailedcount', rfc);
  }

  public GetPasswordResetOTP(logfrm: { loginId: string; sessionCode: string; captchaCode: string; }) {
    return this.http.post(this.baseurl + 'account/reset', logfrm);
  }

  public GetPasswordCheckOTP(logfrm: IOTPPassword) {
    return this.http.post(this.baseurl + 'account/checkotppassword', logfrm);
  }
  public SearchEmployee(text: any)
  { debugger;
    return this.http.post(this.baseurl+'EmpPay/searchEmp',text)
  };
 public GetMenusByUserId(userId:any): Observable<number[]> {
    return this.http.get<number[]>(this.baseurl + `Menu/GetMenusByUserId?userId=${userId}`);
  }
  public GetUserNavmenus(menuCodes: number[]): Observable<INavMenu[]> {debugger;
  const params = new HttpParams({ fromObject: { parentMenuCodes: menuCodes.map(x => x.toString()) } });
  return this.http.get<INavMenu[]>(this.baseurl + 'Menu/GetMenusByHierarchy', { params });
}
  public GetDownloadedContent()
  {   return this.http.get<MostDownloadedContent[]>(this.baseurl + 'ContentDownloadLog/most-downloaded');
  }
  public GetActiveUsersCount(){
   return this.http.get(this.baseurl + 'Member/ActiveUsers');
  }
  public GetTotalcontent(){
    return this.http.get(this.baseurl + 'Content/GetContentCount');
  }
  public GetCountMemberRegisterd(){
     return this.http.get(this.baseurl + 'Member/registered-today/count');
  }
  public GetInstituteAndYear(instituteCode: string | number ) {
    return this.http.get(this.baseurl + 'Year/GetActiveYearsByInstitute?institute_code='+instituteCode);
  }
  public GetInstituteUser(userId: string) {    
     return this.http.get(this.baseurl + 'InstituteUser/GetUserDetails?userId='+userId);
   }
  public GetMailConfigs() {
    return this.http.get(this.baseurl + 'masters/getmailconfigs' );
  }
  public GetMailLogs() {
    return this.http.get(this.baseurl + 'masters/getmaillogs' );
  }
  public GetMailLogList(dtFrom: string,dtTo: string) {
    return this.http.get(this.baseurl + 'masters/getmailloglist?dtFrom=' + dtFrom
        + '&dtTo=' + dtTo);
  }

  public SearchTerm(term: unknown) {
    return this.http.get(this.baseurl + 'masters/searchterm');
  }

  //  ----------------
  //  Autogenerated
  //  -----------------

  public GetAppRolesList() {
    return this.http.get(this.baseurl + 'masters/getapproles');
  }
  public GetAttachmentsList() {
    return this.http.get(this.baseurl + 'masters/getattachments');
  }
 
  public GetDocSeriesList() {
    return this.http.get(this.baseurl + 'masters/getdocseries');
  }
  public GetFormGridHeadersList() {
    return this.http.get(this.baseurl + 'masters/getformgridheaders');
  }
  public GetMailConfigsList() {
    return this.http.get(this.baseurl + 'masters/getmailconfigs');
  }
  public GetMailLogsList() {
    return this.http.get(this.baseurl + 'masters/getmaillogs');
  }
  public GetNavMenusList() {
    return this.http.get(this.baseurl + 'masters/getnavmenus');
  }
  public GetOrganizationsList() {
    return this.http.get(this.baseurl + 'masters/getorganizations');
  }
  public GetRegionsList() {
    return this.http.get(this.baseurl + 'masters/getregions');
  }
  public GetStatesList() {
    return this.http.get(this.baseurl + 'masters/getstates');
  }
  public GetSysDatasList() {
    return this.http.get(this.baseurl + 'masters/getsysdatas');
  }
  public GetUserNavMenusList() {
    return this.http.get(this.baseurl + 'masters/getusernavmenus');
  } 
 
}