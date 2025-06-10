//import { v4 as uuidv4 } from 'uuid';

  export class fileUpload {
    filetype: string;
    attachment: File;
    phyFileName: string = '';
    status: string = '';
  }
   export class IContent 
  {
    content_id: number;
    //bookid: number;
    //filename: string;
    filePath: string;
    filetype: string;
    filesizemb?: number; // optional to match `decimal?` in C#
    //: Date;   // optional to match `DateTime?` in C#
    //uploadedby: string;
    //sourceurl: string;
    //isactive: boolean; 
    title: string;
    author?: string;
    description?: string;
    categoryid: number;
    content_type_id: number;
    publishDate: string;
    thumbnailPath:string;
  }
  export class MostDownloadedContent {
  contentId: number;
  downloadCount: number;
  title: string;
  author: string;
  fileURL: string;
}

  export class ILogin {
    userName: string;
    password: string;
    appMode:'WEB';
    version: string;
  }
  export class IStream{
    streamId :number;
    name:string;
  }
  export class IContentType{
    content_type_id :number;
    name:string;
  }
  export class IClasses {
    classId: number;
    name: string;
    streamId:number;
  }
  export class ISubject {
    subjectId: number;
    name: string;
    streamId:number;
  }
  export class IBook {
    bookId: number;
    title: string;
    author?: string;
    description?: string;
    isbn?: string;
    categoryId: number;
    publishedYear: number;
    coverImagePath?: string;
    createdAt: Date;
  }
  
  export class ICategory{
    categoryid :number;
    categoryname:string;
  }
  export class ILoginOTP {
    userName: string;
    otpCode: string;
    otp: string;
  }

  export class IOTPPassword {
    userType: string;
    userName: string;
    otp: string;
    newPassword: string;
    confirmPassword: string;
    sessionCode: string;
  }

  export class IChangePasswordDto {
    loginId:	string;
    displayName:	string;
    oldPassword:	string;
    newPassword:	string;
    confirmPassword:	string;
    }
  export class IUserToken{
    user_id: string; 
    token: string='';
  }

  export class IUser {
    user_id: string;
   // appRoleName: string;
   // userName: string;
    Email: string;
    Password:string;
   // displayName: string;
    token: string;
   // otpCode: string;
   // changePassword: boolean;
    status?: string;
  }

  export class ICaptcha {
    img: string;
    sessionCode: string;
    signature: string;
  }
  
  export interface retMailConfiglList {
    status: string,
    res: IMailConfig[],
    errormessage: string
  }
  export interface retMailConfigModel {
    status: string,
    res: IMailConfig,
    errormessage: string
  }

//  NavMenus
// export class INavMenu {
//   parId : number;
//   navMenuName : string;
//   description : string;
//   navLink : string;
//   iconClass : string;
//   srNo : number;
//   appRoleCode : string;
//   createdOn : string;
//   createdByName : string;
//   isActive : boolean;
//   uCode : string;
//   sequenceNo : number;
//   extraId1 : number;
//   extraId2 : number;
//   extraValue1 : string;
//   extraValue2 : string;
//   submenus: INavMenu[];
// }
export class INavMenu { 
    menu_code: number;
    menu_type: string;
    menu_description: string;
    parent_menu: number;
    url: string;
    updatedon?: Date; // Optional property
    updatedby: string;
    submenus: INavMenu[];
    isOpen?: boolean=false;
  }

export class ISuperuserNavMenu {
  parId : number;
  navMenuName : string;
  description : string;
  navLink : string;
  iconClass : string;
  srNo : number;
  appRoleCode : string;
  uCode : string;
  sequenceNo : number;
  submenus: ISuperuserNavMenu[];
  selected : boolean;
}

export class IPassPolicy{
  minPasswordLength : number;
   maxPasswordLength : number;
    createdOn : string;
    createdByName : string;
    digitRequired : boolean;
    uppercaseLetterRequired : boolean;
    lowercaseLetterRequired : boolean;
    specialCharacterRequired : boolean;
    uCode : string;
    passwordAgeInDays : number;
    extraId1 : number;
    extraId2 : number;
    extraValue1 : string;
    extraValue2 : string;
      id: any;
 
}

export class IheadersModelModel {
  activeName: string;
  activeDate: string;
 }

export class IChangePassword {
  loginId : string;
  displayName : string;
  oldPassword : string;
  newPassword : string;
  confirmPassword : string;
}

//  Models based on DB

//  ActionLogs
export class IActionLog {
  id : number;
  moduleName : string;
  description : string;
  entityName : string;
  entityValue : string;
  actionName : string;
  clientIP : string;
  clientBrowser : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  AppRoles
export class IAppRole {
  id : number;
  appRoleName : string;
  appRoleCode : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Attachments
export class IAttachment {
  id : number;
  entityName : string;
  entityFieldName : string;
  entityKeyValue : string;
  fileType : string;
  filename : string;
  phyFilename : string;
  uploadedByName : string;
  uploadedOn : string;
  uploadedIP : string;
  userTypeName : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Branches
export class IBranche {
  id : number;
  branchName : string;
  branchCode : string;
  add1 : string;
  add2 : string;
  add3 : string;
  cityName : string;
  stateName : string;
  countryName : string;
  pincode : string;
  cityId : number;
  stateId : number;
  countryId : number;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Cities
export class ICitie {
  id : number;
  cityName : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Countries
export class ICountrie {
  id : number;
  countryName : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Departments
export class IDepartment {
  id : number;
  parId : number;
  departmentName : string;
  departmentCode : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Designations
export class IDesignation {
  id : number;
  designationName : string;
  designationCode : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Divisions
export class IDivision {
  id : number;
  divisionName : string;
  divisionCode : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  EHSTrainings
export class IEHSTraining {
  id : number;
  locationName : string;
  regionName : string;
  plantCode : number;
  topicName : string;
  trainingDate : string;
  startFrom : string;
  endTo : string;
  participantCount : number;
  attendanceFilename : string;
  phyFilename : string;
  officeUserId : number;
  officeUserName : string;
  trainingTitle : string;
  departmentName : string;
  remarks : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  FormGridHeaders
export class IFormGridHeader {
  id : number;
  formName : string;
  tableName : string;
  isPagination : boolean;
  noOfRecords : number;
  canExportExcel : boolean;
  canExportCsv : boolean;
  canUserCustomize : boolean;
  canImportExcel : boolean;
  canImportCsv : boolean;
  officeUserId : number;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  IncidentAccidents
export class IIncidentAccident {
  id : number;
  reportNo : string;
  reportDate : string;
  incidentAccidentTypeName : string;
  accidentDate : string;
  locationName : string;
  regionName : string;
  plantCode : number;
  injuredPerson : string;
  employeeCode : string;
  attendanceFilename : string;
  phyFilename : string;
  officeUserId : number;
  officeUserName : string;
  departmentName : string;
  departmentSupervisorName : string;
  accidentLocation : string;
  accidentNature : string;
  accidentSeverity : string;
  accidentBodyPart : string;
  accidentDescription : string;
  remarks : string;
  whyAnalysis : string;
  pPERequired : string;
  pPEUsed : string;
  accidentCause : string;
  accidentImmediateCause : string;
  accidentRootCause : string;
  correctiveAction : string;
  preventiveAction : string;
  riskAssessment : string;
  correctiveActionRemarks : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  InductionTrainings
export class IInductionTraining {
  id : number;
  locationName : string;
  regionName : string;
  plantCode : number;
  trainingDate : string;
  startFrom : string;
  endTo : string;
  participantCount : number;
  attendanceFilename : string;
  phyFilename : string;
  officeUserId : number;
  officeUserName : string;
  trainingTitle : string;
  departmentName : string;
  remarks : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Locations
export class ILocation {
  id : number;
  locationName : string;
  locationCode : string;
  locationTypeName : string;
  countryName : string;
  stateName : string;
  territoryName : string;
  regionName : string;
  countryId : number;
  stateId : number;
  territoryId : number;
  regionId : number;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}
export class IDept
{
  institute_code:number;
  dept_code:number;
  dept_name:string;
  user_idn:string;
}
export class IDesg
{
  institute_code:number;
  desg_code:number;
  desg_desc:string;
  user_idn:string;
}
export class IBank
 {
  institute_code: number;
  bank_code: number;
  bank_name: string;      // Required
  branch_name?: string;   // Optional
  salary_ac_no?: string;  // Optional
  location?: string;      // Optional
  user_idn: string;       // Required  
}
export class IProfTax {
  institute_code: number;
  sr_no: number;
  from_amt: number;
  to_amt: number ;
  proftax: number = 0;
  applicable_from_date: Date;
  user_idn: string;
  category: string;
}
export class IDaMast{
  institute_code: number;
  sr_no: number;
  from_amt: number = 0;
  to_amt: number = 0;
  da_amt: number = 0;
  da_percent: number = 0;
  category: string;
  applicable_from_date: Date;
  user_idn: string;
}
export class IHraMast{
  institute_code: number;
  sr_no: number;
  from_amt: number = 0;
  to_amt: number = 0;
  hra_amt: number = 0;
  category: string;
  applicable_from_date: Date;
  user_idn: string;
}
export class ITaMast{
  institute_code: number;
  sr_no: number;
  from_amt: number = 0;
  to_amt: number = 0;
  ta_amt: number = 0;
  applicable_from_date: Date;
  user_idn: string;
}
export class EmpPersonal {
  institute_code: number;
  emp_code: number;
  local_add1: string = " ";
  local_add2: string = " ";
  local_add3: string = " ";
  local_city: string = " ";
  local_phone_no: string = " ";
  perm_add1: string = " ";
  perm_add2: string = " ";
  perm_add3: string = " ";
  perm_city: string = " ";
  perm_phone_no: string = " ";
  email: string = " ";
  driving_lic_no: string = " ";
  blood_group: string = " ";
  emp_religion: string = " ";
  emp_caste: string = " ";
  marital_status: string = "S";
  spouse_name: string = " ";
  no_of_incumbents: number = 0;
  hometown: string = " ";
  nearest_rail_stn: string = " ";
  no_of_family_members: number = 0;
  subject_name: string = " ";
  no_of_lectures: number = 0;
  birth_date: Date | null = null;
  pan_no: string = " ";
  qualification: string = " ";
  class_obtained: string = " ";
  year_of_passing: string = " ";
  univ_board: string = " ";
  pre_college_nm: string = "";
  pre_from_date: Date | null = null;
  pre_to_date: Date | null = null;
  pre_full_part_time: string = " ";
  pre_univ_board: string = " ";
  degree: string = " ";
  degree_year: string = " ";
  degree_percent: string = " ";
  post_graduation: string = " ";
  pg_year: string = " ";
  pg_percent: string = " ";
  mphil: string = " ";
  mphil_year: string = " ";
  mphil_percent: string = " ";
  phd: string = " ";
  phd_year: string = " ";
  phd_percent: string = " ";
  other_qualification: string = " ";
  other_qua_year: string = " ";
  other_qua_percent: string = " ";
  user_idn: string | null = null;
  last_update: Date;
  adhar_no: string = " ";
}

export class IClaMast{
  institute_code: number;
  sr_no: number;
  from_amt: number = 0;
  to_amt: number = 0;
  cla_amt: number = 0;
  applicable_from_date: Date;
  user_idn: string;
}
export class IPfMast {
  institute_code: number;
  sr_no: number;
  max_pf: number = 0;
  pf_percent: number = 0;
  pension_perc: number = 0;
  max_pension: number = 0;
  applicable_from_date: Date;
  user_idn: string;
}
//  MailConfigs
export class IMailConfig {
  id : number;
  mailAction : string;
  mailDescription : string;
  mailFromName : string;
  mailFromEmail : string;
  mailFromPwd : string;
  mailBcc : string;
  mailCc : string;
  mailSubject : string;
  mailContent : string;
  smtpServer : string;
  smtpPort : number;
  isAuthenticationReq : boolean;
  isSslReq : boolean;
  isHtml : boolean;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  MailLogs
export class IMailLog {
  id : number;
  mailQueuedOn : string;
  mailStatus : string;
  mailConfigId : number;
  mailFromName : string;
  mailFromEmail : string;
  mailFromPwd : string;
  mailTo : string;
  mailBcc : string;
  mailCc : string;
  mailSubject : string;
  mailContent : string;
  smtpServer : string;
  smtpPort : number;
  isAuthenticationReq : boolean;
  isSslReq : boolean;
  isHtml : boolean;
  mailError : string;
  entityTypeName : string;
  entityId : number;
  secondaryEntityTypeName : string;
  secondaryEntityId : number;
  mailAttachment : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}
export class ISupplimentary
{
  institute_code: number;          
  year_code: number;              
  srno: number;                    
  supplementary_date: Date;       
  description: string;            
  effect_itax: string;              
  user_idn: string;      
  sr_no: any;
}
export class ISupplimentaryTransaction
{
  year_code:number;
  month:number;
  institute_code: number;
  emp_code: number;
  sal_date: string;
  srno: number;
  present_days: number;
  leave_without_pay: number;
  half_pay: number;
  dept_code: number;
  desg_code: number;
  bank_code: number;
  bank_ac_no: string;
  startscale: number;
  basic: number;
  calc_basic: number;
  pp: number;
  da: number;
  calc_dan: number;
  cla: number;
  calc_cla: number;
  hra: number;
  calc_hran: number;
  ta: number;
  calc_ta: number;
  allowance1_code: number;
  allowance1_amt: number;
  allowance2_code: number;
  allowance2_amt: number;
  allowance3_code: number;
  allowance3_amt: number;
  sp_allowance: number;
  daarr_cash: number;
  misc_earning: number;
  gross_earningo: number;
  i_tax: number;
  co_op_soc_fees: number;
  patpedhi_4: number;
  patpedhi_2: number;
  pf_old: number;
  pf_no_varchar: number;
  pf_wagen: number;
  pf_loan: number;
  pf_loan_install_no: string;
  employer_pf: number;
  employer_pension: number;
  ptax: number;
  lic: number;
  lic_2_sec88: number;
  lic_3_sec80: number;
  festival_advance: number;
  recovery1_amt: number;
  recovery1_desc: string;
  recovery2_amt: number;
  recovery2_desc: string;
  recovery3_amt: number;
  recovery3_desc: string;
  recovery_basic: number;
  recovery_da: number;
  recovery_hra: number;
  recovery_cla: number;
  recovery_ta: number;
  recovery_misc: number;
  salary_recovery: number;
  misc_deduction: number;
  daarr_pf: number;
  gross_deduction: number;
  net_payo?: number;
  manual_modified: string;
  revenue_stamp: number;
  user_idn: string;
  last_update: string;
  add_pf: number;
  lic3: string;
  lic2: string;
  lic1: string;
  dapay: number;
  calc_dapayn: number;
  recovery_dp: number;
  itax_paid: number;
  gross_earning: number;
  net_pay: number;
  calc_da: number;
  calc_hra: number;
  calc_dapay: number;
  pf_wage: number;
  patpedhi_1: number;
  pf_no: string;
  pf_noint: number;
  dcps_no: string;
  dcps_amt: number;
  pf: number;
  taarr_cash: number;
  rd: number;
  rdtotal: number;
  rdint: number;
  pf_no_old: number;
}
export class ISupplimentaryTransactionList
{
  institute_code?: number;          
  year_code: number;              
  srno: number;                    
  supplementary_date?: Date;  
  month?:string;     
  description?: string;            
  effect_itax?: string;              
  user_idn?: string;  
  newrec?:string;
  list?:string;
}
//  OfficeUsers
export class IOfficeUser {
  id : number;
  officeUserName : string;
  officeUserCode : string;
  designationName : string;
  locationName : string;
  locationCode : string;
  locationTypeName : string;
  workEmail : string;
  alternateEmail : string;
  mobileNo : string;
  alternateMobileNos : string;
  appRoleName : string;
  appRoleCode : string;
  isAdmin : boolean;
  loginId : string;
  lastLogin : string;
  failedLoginCount : number;
  appUserId : string;
  appRoleId : number;
  locationId : number;
  regionName : string;
  regionId : number;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Organizations
export class IOrganization {
  id : number;
  organizationName : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Regions
export class IRegion {
  id : number;
  regionName : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  States
export class IState {
  id : number;
  stateName : string;
  shortCode : string;
  stateGSTCode : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  SysDatas
export class ISysData {
  id : number;
  fieldName : string;
  fieldValue : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  UnsafeActConditions
export class IUnsafeActCondition {
  id : number;
  typeName : string;
  filename : string;
  phyFileName : string;
  officeUserName : string;
  officeUserId : number;
  departmentName : string;
  remarks : string;
  briefInfo : string;
  latitude : number;
  longitude : number;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  UserNavMenus
export class IUserNavMenu {
  id : number;
  navMenuId : number;
  appUserId : string;
  formOptions : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  Zones
export class IZone {
  id : number;
  zoneName : string;
  zoneCode : string;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}

//  FormGridDetails
export class IFormGridDetail {
  id : number;
  formGridHeaderId : number;
  fieldName : string;
  fieldHeading : string;
  isSortable : boolean;
  isFilterable : boolean;
  isResizable : boolean;
  isVisible : boolean;
  isEditable : boolean;
  isPinned : boolean;
  position : number;
  width : number;
  fieldType : string;
  fieldFormat : string;
  headerBackColor : string;
  headerForeColor : string;
  columnBackColor : string;
  columnForeColor : string;
  rowBackColor : string;
  rowForeColor : string;
  rowHighlightedColor : string;
  selectedRowHighlightedColor : string;
  canExportExcel : boolean;
  canExportCsv : boolean;
  canImportExcel : boolean;
  canImportCsv : boolean;
  isCompulsoryForImport : boolean;
  createdOn : string;
  createdByName : string;
  isActive : boolean;
  uCode : string;
  sequenceNo : number;
  extraId1 : number;
  extraId2 : number;
  extraValue1 : string;
  extraValue2 : string;
}
export interface Resource {
  type: 'ebook' | 'video';
  title: string;
  description: string;
  link: string;
  embedLink?: string; // Optional, for videos with embed codes
}


