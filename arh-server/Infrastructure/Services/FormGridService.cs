using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Identity;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Infrastructure.Data;
using Infrastructure.Utility;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;

namespace Infrastructure.Services
{
    public class FormGridService<T> : IFormGridService<T> where T : new()
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMastersService _ms;

        public FormGridService()
        {
        }

        public FormGridService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public FormGridService(IUnitOfWork unitOfWork, IMastersService ms)
        {
            _unitOfWork = unitOfWork;
            _ms = ms;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            return _unitOfWork;
        }

        public async Task<IReadOnlyList<FormGridDetail>> GetFormGridDetails(string FormName, int UserId, string AppRoleCode = null)
        {
            string[] validForms = new string[] {
                "AppRole",
                "Attachment",
                "City",
                "Country",
                "Department",
                "Designation",
                "Medicine",
                "patient",
                "Rate",
                "MailConfig",
                "MailLog",
                "Organization",
                "State",
                "UserSecurity",
                "CompanyAdmin",
                "ActionLog",
                "Appointment"
            };

            if (!validForms.Contains(FormName))
            {
                return null;
            }

           Type tabType = typeof(T);
     
            var spec = new FormGridHeaderSpecification(FormName, 0);

            FormGridHeader fgh = await _unitOfWork.Repository<FormGridHeader>()
                    .GetEntityWithSpec(spec);

            if (UserId != -1 && fgh == null)
            {
                UserId = -1;

                spec = new FormGridHeaderSpecification(FormName, -1);
                fgh = await _unitOfWork.Repository<FormGridHeader>()
                    .GetEntityWithSpec(spec);
            }
            if (fgh == null)
            {
                //  There is no header table, create it
                fgh = new FormGridHeader();
                fgh.IsActive = true;
                fgh.CanExportCsv = true;
                fgh.CanExportExcel = true;
                fgh.CanImportExcel = true;
                fgh.CanUserCustomize = true;
                fgh.CreatedById = UserId;
                fgh.CreatedOn = DateTime.UtcNow;
                fgh.FormName = FormName;
                fgh.IsDeleted = false;
                fgh.IsPagination = true;
                fgh.NoOfRecords = 100;
                fgh.OfficeUserId = -1;     //  Factory setting
                fgh.TableName = tabType.Name;   // DataSource;
                fgh.AppRoleCode = "ADMIN";
                fgh.CreatedByName = "ADMIN";
                fgh.ExtraValue1 = "abc";
                fgh.ExtraValue2 = "abc";
                fgh.ExtraId1 = 0;
                fgh.ExtraId2 = 0;
                fgh.LogHistory = "";
                fgh.JsonData = "{}";
                

                _unitOfWork.Repository<FormGridHeader>().Add(fgh);
                await _unitOfWork.Complete();

                int pos = 0;
                //Type tabType = Assembly.GetExecutingAssembly().GetType(DataSource);
                if (tabType != null)
                {
                    foreach (PropertyInfo pi in tabType.GetProperties())
                    {
                        if (pi.Name != "ErrorMessage") 
                        {
                            FormGridDetail fgd = new FormGridDetail()
                            {
                                FieldHeading = pi.Name.Replace("Is", ""),
                                FieldType = "",
                                FieldName = pi.Name
                            };
                            fgd.FormGridHeaderId = fgh.Id;
                            fgd.FieldName = pi.Name;                            
                            fgd.FieldFormat = GetFieldFormatFromType(pi.PropertyType); 
                        if (pi.Name == "IsDeleted" || pi.Name == "IsActive")
                        {
                            fgd.FieldHeading = pi.Name.Replace("Is", "");
                        }
                        else
                        {
                            fgd.FieldHeading = pi.Name;
                        }
                        //  fgd.colid = pi.Name;
                        fgd.IsEditable = false;
                        fgd.IsFilterable = true;
                        fgd.IsPinned = false;
                        fgd.IsResizable = true;
                        fgd.IsSortable = true;

                        if (fgd.FieldName.ToLower().EndsWith("id") || pi.Name.StartsWith("ExtraId") || pi.Name.StartsWith("ExtraValue") 
                                    || pi.Name == "UCode" || pi.Name == "SequenceNo"
                                    || pi.Name == "LogHistory" || pi.Name == "IsExisting" || pi.Name == "IsDeleted" || pi.Name == "IsUpdated")
                        {
                            fgd.CanExportCsv = false;
                            fgd.CanExportExcel = false;
                            fgd.CanImportExcel = false;
                            fgd.CanImportCsv = false;
                            fgd.IsCompulsoryForImport = false;
                            fgd.IsEditable = false;
                            fgd.IsFilterable = false;
                            fgd.IsPinned = false;
                            fgd.IsResizable = false;
                            fgd.IsSortable = false;

                            fgd.IsVisible = false;
                        }
                        else
                        {
                            fgd.CanExportCsv = true;
                            fgd.CanExportExcel = true;
                            fgd.CanImportExcel = true;
                            fgd.CanImportCsv = true;
                            fgd.IsCompulsoryForImport = true;

                            fgd.IsVisible = true;
                        }
                        //fgd.officeexecid = -1;
                        fgd.Position = (++pos) * 10;
                        fgd.CreatedById = UserId;
                        fgd.CreatedByName = "Admin";
                        fgd.CreatedOn = DateTime.UtcNow;
                        fgd.ExtraValue1 = "abc";
                        fgd.ExtraValue2 = "abc";
                        fgd.JsonData = "{}";
                        fgd.LogHistory = "nn";
                        fgd.FieldType = pi.PropertyType.Name.ToLower();
                        if (fgd.FieldType == "string")
                            fgd.Width = 150;
                        else if (fgd.FieldType == "datetime")
                            fgd.Width = 100;
                        else if (fgd.FieldType.StartsWith("bool"))
                        {
                            fgd.FieldFormat = "Yes | No";
                            fgd.Width = 75;
                        }
                        else
                            fgd.Width = 100;

                        
                        _unitOfWork.Repository<FormGridDetail>().Add(fgd);
                        }
                    }
                    await _unitOfWork.Complete();
                }
            }
            else
            {
                await this.RefreshFormGridDetails(FormName, -1);
            }

            IReadOnlyList<FormGridDetail> fgds = await _unitOfWork.Repository<FormGridDetail>()
                    .ListAsync(new BaseSpecification<FormGridDetail>(x => x.FormGridHeaderId == fgh.Id));

            return fgds.OrderBy(x => x.Position).ToList();
        }

        public async Task<IImportExcelData<T>> ImportExcelData(string FormName, AppUser appUser, string fileName, List<string> ExtraFields=null, string datefields=null)
        {
            string frmName = FormName;
            IImportExcelData<T> ret = new ImportExcelData<T>();

            try
            {
                using (var wb = new XLWorkbook(fileName))
                {
                    var currentSheet = wb;
                    var workSheet = currentSheet.Worksheet(1);
                    var noOfCol = workSheet.ColumnsUsed().Count();
                    var noOfRow = workSheet.RowsUsed().Count();

                    FormGridService<T> fgs = new FormGridService<T>(_unitOfWork);
                    var ulfds = await fgs.GetFormGridDetails(FormName, appUser.OfficeUserId);
                    if (ExtraFields != null && ExtraFields.Count > 0)
                    {
                        foreach (var ef in ExtraFields)
                        {
                            ulfds.Append(new FormGridDetail() {
                                FieldHeading = ef,
                                FieldName = ef,
                                FieldType="string",
                                IsCompulsoryForImport = false
                            });
                        }
                    }
                    String headingName = "";
                    int rowno = 1;

                    //  Check Header validity
                    for (int colIterator = 1; colIterator <= noOfCol; ++colIterator)
                    {
                        if (workSheet.Cell(rowno, colIterator).TryGetValue<string>(out headingName))
                        {
                            //  headingName = workSheet.Cell(rowno, colIterator).Value.ToString();
                            var ulfd = ulfds.Where(x => x.FieldHeading == headingName).FirstOrDefault();
                            if (ulfd != null)
                            {
                                ret.Headings.Add(new FormGridDetail()
                                {
                                    Position = colIterator,
                                    FieldHeading = headingName,
                                    FieldName = ulfd.FieldName,
                                    FieldType = ulfd.FieldType,
                                    FieldFormat = ulfd.FieldFormat,
                                    IsCompulsoryForImport = ulfd.IsCompulsoryForImport
                                });
                            }
                        }
                    }
                    if (ret.Headings.Count <= 0)
                    {
                        ret.ErrorMessage = "No valid headers found";
                        return ret;
                    }

                    for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                    {
                        //if (!String.IsNullOrEmpty(retdata))
                        //    retdata += ", ";

                        //retdata += "{ ";

                        var installbase = new T();
                        var itype =  typeof(T);

                        foreach (FormGridDetail k in ret.Headings)
                        {
                            var dataType = workSheet.Cell(rowIterator, (int)k.Position).DataType;
                            var objval = workSheet.Cell(rowIterator, (int)k.Position).Value;

                            if (dataType != XLDataType.Blank && !objval.IsBlank)
                            {
                                PropertyInfo prop = itype
                                    .GetProperties()
                                    .Where(x => x.Name == k.FieldName)
                                    .FirstOrDefault();

                                if (prop.PropertyType.Name == "String")
                                {
                                    if (objval.IsText)
                                    {
                                        prop.SetValue(installbase, objval.ToString(), null);
                                    }
                                    if (objval.IsDateTime)
                                    {
                                        prop.SetValue(installbase, objval.ToString(), null);
                                    }
                                    if (objval.IsNumber)
                                    {
                                        if (!String.IsNullOrEmpty(datefields) && datefields.Contains(k.FieldName+","))
                                        {
                                            DateTime tmpdate = DateTime.FromOADate(Convert.ToDouble(objval));
                                            prop.SetValue(installbase, string.Format("{0:yyyy-MM-dd}", tmpdate), null);
                                            //prop.SetValue(installbase, string.Format("{0:dd-MM-yyyy}", tmpdate), null);
                                        }
                                        else if (!String.IsNullOrEmpty(objval.ToString()))
                                        {
                                            prop.SetValue(installbase, objval.ToString(), null);
                                        }
                                    }
                                    // else
                                    //     prop.SetValue(installbase, objval.ToString(), null);
                                }
                                if (prop.PropertyType.FullName.Contains("System.DateTime"))
                                {
                                    if (!String.IsNullOrEmpty(objval.ToString()))
                                    {
                                        DateTime d = DateTime.UtcNow;  // DateTime.Parse(objval.ToString());
                                        if (DateTime.TryParse(objval.ToString(), out d))
                                        {
                                            prop.SetValue(installbase, (d), null);
                                        }
                                        else
                                        {
                                            ret.ErrorInFields.Add(new ExcelUploadError()
                                            {
                                                errormessage = "Error in data",
                                                fieldname = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(k.FieldName),
                                                rowno = rowIterator - 2
                                            });
                                        }
                                    }
                                }
                                if (prop.PropertyType.FullName.Contains("System.Int32"))
                                {
                                    try
                                    {
                                        prop.SetValue(installbase, Convert.ToInt32(objval), null);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Exception: " + ex.Message);

                                        ret.ErrorInFields.Add(new ExcelUploadError()
                                        {
                                            errormessage = ex.Message,
                                            fieldname = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(k.FieldName),
                                            rowno = rowIterator - 2
                                        });
                                    }
                                }
                                if (prop.PropertyType.FullName.Contains("System.Boolean"))
                                {
                                    if (!String.IsNullOrEmpty(objval.ToString()))
                                    {
                                        if (objval.ToString().ToUpper() == "YES" || objval.ToString().ToUpper() == "TRUE" || objval.ToString().ToUpper() == "ACTIVE")
                                        {
                                            prop.SetValue(installbase, true, null);
                                        }
                                        else
                                        {
                                            prop.SetValue(installbase, false, null);
                                        }
                                    }
                                }
                                if (prop.PropertyType.FullName.Contains("System.Decimal"))
                                {
                                    try
                                    {
                                        prop.SetValue(installbase, Convert.ToDecimal(objval), null);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Exception: " + ex.Message);
                                        ret.ErrorInFields.Add(new ExcelUploadError()
                                        {
                                            errormessage = ex.Message,
                                            fieldname = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(k.FieldName),
                                            rowno = rowIterator - 2
                                        });
                                    }
                                }
                            }
                            else
                            {
                                if (k.IsCompulsoryForImport == true)
                                {
                                    // errorstatus = "Error";
                                    // retdata += "\"" + k.fieldname + "\":{\"v\":\"\",\"e\":\"Data Required\"},";

                                    ret.ErrorInFields.Add(new ExcelUploadError()
                                    {
                                        errormessage = "Data Required",
                                        fieldname = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(k.FieldName),
                                        rowno = rowIterator - 2
                                    });
                                }
                            }
                        }

                        ret.DataSource.Add(installbase);
                    }
                }
            }
            catch (Exception ex)
            {
                ret.ErrorMessage = "Error occurred. Error details: " + ex.Message;
                return ret;
            }

            ret.UploadCode = Guid.NewGuid().ToString();

            return ret;

        }

        public async Task<Byte[]> GetTemplateData(string FormName, AppUser ou, Dictionary<string, string> DefaultValues = null, List<string> ExtraFields=null)
        {
            var fghl = await _unitOfWork.Repository<FormGridHeader>()
                        .ListAsync(new BaseSpecification<FormGridHeader>(x => x.FormName == FormName));
            
            var fgh = fghl.Where(x => x.OfficeUserId == ou.OfficeUserId).FirstOrDefault();
            if (fgh == null)
            {
                fgh = fghl.Where(x => x.OfficeUserId == -1).FirstOrDefault();
            }

            var fgds = (await _unitOfWork.Repository<FormGridDetail>()
                        .ListAsync(new BaseSpecification<FormGridDetail>(x => x.FormGridHeaderId == fgh.Id && x.CanImportExcel == true)))
                    .OrderBy(x => x.Position);

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    //Here setting some document properties

                    workbook.Properties.Author = "Leegan Softwares";
                    workbook.Properties.Title = "Template - " + FormName;

                    //set the workbook properties and add a default sheet in it
                    var ws = workbook.Worksheets.Add("Sheet1");

                    ws.Name = "Template"; //Setting Sheet's name
                    ws.Style.Font.FontSize = 11; //Default font size for whole sheet
                    ws.Style.Font.FontName = "Calibri"; //Default Font name for whole sheet

                    XLColor colFromHex = XLColor.AirForceBlue;  //  ("#A6CE96");

                    var modelCells = ws.Cell("A1");

                    string modelRange = "";

                    int EcportedCount = fgds.Count();
                    if (ExtraFields != null && ExtraFields.Count > 0)
                    {
                        EcportedCount += ExtraFields.Count;
                    }
                    if (EcportedCount != 0)
                    {
                        ws.Range(1, 1, 1, EcportedCount).Style.Fill.SetPatternType(XLFillPatternValues.Solid);
                        ws.Range(1, 1, 1, EcportedCount).Style.Fill.SetBackgroundColor(colFromHex);
                        ws.Range(1, 1, 1, EcportedCount).Style.Font.Bold = true;
                    }

                    int colno = 0;
                    foreach (var fgd in fgds)
                    {
                        colno++;

                        ws.Cell(1, colno).Value = fgd.FieldHeading;
                        if (fgd.IsCompulsoryForImport)
                        {
                            ws.Cell(2, colno).Value = "Required";
                        }
                        if (DefaultValues != null && DefaultValues.Count > 0)
                        {
                            if (DefaultValues.ContainsKey(fgd.FieldName))
                            {
                                ws.Cell(3, colno).Value = DefaultValues.GetValueOrDefault(fgd.FieldName);
                            }
                        }

                        if (!String.IsNullOrEmpty(fgd.FieldType) && !String.IsNullOrEmpty(fgd.FieldFormat) && (fgd.FieldFormat.Contains("dd-mm-yyyy") || fgd.FieldType == "datetime"))
                        {
                            ws.Column(colno).Style.DateFormat.Format = "dd-mm-yyyy";
                        }
                    }
                    if (ExtraFields != null && ExtraFields.Count > 0)
                    {
                        foreach (var ef in ExtraFields)
                        {
                            colno++;

                            ws.Cell(1, colno).Value = ef;
                        }
                    }

                    var modelTable = ws.Cell(modelRange);

                    // Assign borders
                    modelTable.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                    modelTable.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
                    modelTable.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
                    modelTable.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

                    ws.Columns().AdjustToContents();

                    MemoryStream ms = new MemoryStream();
                    workbook.SaveAs(ms);
                    Byte[] bin = ms.ToArray();

                    return bin;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        public async Task<Byte[]> GetFormDataDownload(string FormName, AppUser appUser, IReadOnlyList<T> data)
        {
            var fghl = await _unitOfWork.Repository<FormGridHeader>()
                        .ListAsync(new BaseSpecification<FormGridHeader>(x => x.FormName == FormName));
            
            var fgh = fghl.Where(x => x.OfficeUserId == appUser.OfficeUserId).FirstOrDefault();
            if (fgh == null)
            {
                fgh = fghl.Where(x => x.OfficeUserId == -1).FirstOrDefault();
            }

            var fgds = (await _unitOfWork.Repository<FormGridDetail>()
                        .ListAsync(new BaseSpecification<FormGridDetail>(x => x.FormGridHeaderId == fgh.Id && x.CanExportExcel == true)))
                    .OrderBy(x => x.Position);

            try
            {
                using (var workbook = new XLWorkbook())
                {
                    //Here setting some document properties
                    workbook.Properties.Author = "Leegan Softwares";
                    workbook.Properties.Title = "Download - " + FormName;

                    //set the workbook properties and add a default sheet in it
                    var ws = workbook.Worksheets.Add("Sheet1");

                    ws.Name = "Download"; //Setting Sheet's name
                    ws.Style.Font.FontSize = 11; //Default font size for whole sheet
                    ws.Style.Font.FontName = "Calibri"; //Default Font name for whole sheet

                    //  System.Drawing.Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#A6CE96");
                    XLColor colFromHex = XLColor.AirForceBlue;  //  ("#A6CE96");

                    var modelCells = ws.Cell("A1");

                    string modelRange = "";

                    int EcportedCount = fgds.Count();
                    if (EcportedCount != 0)
                    {
                        ws.Range(1, 1, 1, EcportedCount).Style.Fill.SetPatternType(XLFillPatternValues.Solid);
                        ws.Range(1, 1, 1, EcportedCount).Style.Fill.SetBackgroundColor(colFromHex);
                        ws.Range(1, 1, 1, EcportedCount).Style.Font.Bold = true;
                    }

                    int colno = 0;
                    foreach (var fgd in fgds)
                    {
                        colno++;

                        ws.Cell(1, colno).Value = fgd.FieldHeading;

                        if (!String.IsNullOrEmpty(fgd.FieldType) && !String.IsNullOrEmpty(fgd.FieldFormat) && (fgd.FieldFormat.Contains("dd-mm-yyyy") || fgd.FieldType == "datetime"))
                        {
                            ws.Column(colno).Style.DateFormat.Format = "dd-mm-yyyy";
                        }
                    }

                    var modelTable = ws.Cell(modelRange);

                    // Assign borders
                    modelTable.Style.Border.SetTopBorder(XLBorderStyleValues.Thin);
                    modelTable.Style.Border.SetLeftBorder(XLBorderStyleValues.Thin);
                    modelTable.Style.Border.SetRightBorder(XLBorderStyleValues.Thin);
                    modelTable.Style.Border.SetBottomBorder(XLBorderStyleValues.Thin);

                    ws.Columns().AdjustToContents();

                    var itype =  typeof(T);

                    int rowIndex = 2;
                    foreach (var d in data)
                    {
                        colno = 0;
                        foreach (var fgd in fgds)
                        {
                            colno++;

                            try
                            {
                                PropertyInfo pi = itype.GetProperty(fgd.FieldName);
                                pi.GetValue(d, null);

                                // PropertyInfo prop = itype
                                //         .GetProperties()
                                //         .Where(x => x.Name == fgd.FieldName)
                                //         .FirstOrDefault();
                                

                                var objval = pi.GetValue(d, null);  // prop.GetValue(d, null);

                                if (pi.PropertyType.FullName.Contains("System.DateTime"))
                                {
                                    if (objval != null)
                                    {
                                        ws.Cell(rowIndex, colno).Value = (DateTime) objval ;
                                    }
                                }
                                else if (pi.PropertyType.FullName.Contains("System.Boolean"))
                                {
                                    if (objval != null)
                                    {
                                        if (Convert.ToBoolean(objval))
                                        {
                                            ws.Cell(rowIndex, colno).Value = "Yes";
                                        }
                                        else
                                        {
                                            ws.Cell(rowIndex, colno).Value = "No";
                                        }
                                    }
                                }
                                else if (pi.PropertyType.FullName.Contains("System.Int32"))
                                {
                                    if (objval != null)
                                    {
                                        ws.Cell(rowIndex, colno).Value = (int) objval;
                                    }
                                }
                                else if (pi.PropertyType.FullName.Contains("System.Decimal"))
                                {
                                    if (objval != null)
                                    {
                                        ws.Cell(rowIndex, colno).Value = (decimal) objval;
                                    }
                                }
                                else
                                    ws.Cell(rowIndex, colno).Value = objval.ToString();

                                // if (!String.IsNullOrEmpty(fgd.FieldType) && !String.IsNullOrEmpty(fgd.FieldFormat) && (fgd.FieldFormat.Contains("dd-mm-yyyy") || fgd.FieldType == "datetime"))
                                // {
                                //     ws.Column(colno).Style.Numberformat.Format = "dd-mm-yyyy";
                                // }
                            }
                            catch(Exception exx)
                            {
                                Console.WriteLine(exx.Message);    
                            }
                        }
                        ++rowIndex;
                    }

                    MemoryStream ms = new MemoryStream();
                    workbook.SaveAs(ms);
                    Byte[] bin = ms.ToArray();

                    return bin;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }

        public async Task<FormGridDetail> UpdateFormGridDetail(string FormName, FormGridDetail FgdData, string UserName)
        {
            //var u = await _unitOfWork.Repository<OfficeUser>().GetByNameAsync("OfficeUserName", UserName);

            var fghl = await _unitOfWork.Repository<FormGridHeader>()
                .ListAsync(new BaseSpecification<FormGridHeader>(x => x.FormName == FormName && x.OfficeUserId == -1));

            var fgh = fghl.FirstOrDefault();

            var fgd = await _unitOfWork.Repository<FormGridDetail>().GetEntityWithSpec(new BaseSpecification<FormGridDetail>(
                x => x.FieldName == FgdData.FieldName && x.FormGridHeaderId == fgh.Id
            ));

            if (fgd != null) 
            {
                fgd.FieldHeading = FgdData.FieldHeading;
                fgd.Width = FgdData.Width;
                fgd.Position = FgdData.Position;
                fgd.FieldFormat = FgdData.FieldFormat;
                fgd.IsCompulsoryForImport = FgdData.IsCompulsoryForImport;
                fgd.IsEditable = FgdData.IsEditable;
                fgd.IsFilterable = FgdData.IsFilterable;
                fgd.IsResizable = FgdData.IsResizable;
                fgd.IsSortable = FgdData.IsSortable;
                fgd.IsVisible = FgdData.IsVisible;
                fgd.CanExportCsv = FgdData.CanExportCsv;
                fgd.CanExportExcel = FgdData.CanExportExcel;
                fgd.CanImportCsv = FgdData.CanImportCsv;
                fgd.CanImportExcel = FgdData.CanImportExcel;

                _unitOfWork.Repository<FormGridDetail>().Update(fgd);

                await _unitOfWork.Complete();
            }

            return fgd;
        }

        public async Task<IReadOnlyList<FormGridDetail>> RefreshFormGridDetails(string FormName, int UserId)
        {
            Type tabType = typeof(T);

            var spec = new FormGridHeaderSpecification(FormName, UserId);

            FormGridHeader fgh = await _unitOfWork.Repository<FormGridHeader>()
                    .GetEntityWithSpec(spec);

            if (UserId != -1 && fgh == null)
            {
                UserId = -1;

                spec = new FormGridHeaderSpecification(FormName, -1);
                fgh = await _unitOfWork.Repository<FormGridHeader>()
                    .GetEntityWithSpec(spec);
            }
            if (fgh != null)
            {
                var fgdl = await _unitOfWork.Repository<FormGridDetail>()
                    .ListAsync(new BaseSpecification<FormGridDetail>(x => x.FormGridHeaderId == fgh.Id));

                string[] ExcludeFields = {
                    "Id", "ExtraId", "ExtraValue",
                    "LogHistory", "IsExisting", "IsDeleted", "IsUpdated",
                    "UCode", "SequenceNo", 
                    "ErrorMessage", "Errors"
                };
                int pos = fgdl.Max(x => x.Position) + 10;
                //Type tabType = Assembly.GetExecutingAssembly().GetType(DataSource);
                if (tabType != null)
                {
                    foreach (PropertyInfo pi in tabType.GetProperties())
                    {
                        if (!ExcludeFields.Contains(pi.Name) && fgdl.Where(x => x.FieldName == pi.Name).Count() <= 0) 
                        {
                            FormGridDetail fgd = new FormGridDetail()
                            {
                                FieldHeading = "",
                                FieldName = pi.Name,
                                FieldType = ""
                            };

                            fgd.FormGridHeaderId = fgh.Id;
                            fgd.FieldName = pi.Name;
                            if (pi.Name == "IsDeleted" || pi.Name == "IsActive")
                            {
                                fgd.FieldHeading = pi.Name.Replace("Is", "");
                            }
                            else
                            {
                                fgd.FieldHeading = pi.Name;
                            }
                            //  fgd.colid = pi.Name;
                            fgd.IsEditable = false;
                            fgd.IsFilterable = true;
                            fgd.IsPinned = false;
                            fgd.IsResizable = true;
                            fgd.IsSortable = true;

                            if (fgd.FieldName.ToLower().EndsWith("id") || pi.Name.StartsWith("ExtraId") || pi.Name.StartsWith("ExtraValue") 
                                        || pi.Name == "UCode" || pi.Name == "SequenceNo"
                                        || pi.Name == "LogHistory" || pi.Name == "IsExisting" || pi.Name == "IsDeleted" || pi.Name == "IsUpdated")
                            {
                                fgd.CanExportCsv = false;
                                fgd.CanExportExcel = false;
                                fgd.CanImportExcel = false;
                                fgd.CanImportCsv = false;
                                fgd.IsCompulsoryForImport = false;
                                fgd.IsEditable = false;
                                fgd.IsFilterable = false;
                                fgd.IsPinned = false;
                                fgd.IsResizable = false;
                                fgd.IsSortable = false;

                                fgd.IsVisible = false;
                            }
                            else
                            {
                                fgd.CanExportCsv = true;
                                fgd.CanExportExcel = true;
                                fgd.CanImportExcel = true;
                                fgd.CanImportCsv = true;
                                fgd.IsCompulsoryForImport = true;

                                fgd.IsVisible = true;
                            }
                            //fgd.officeexecid = -1;
                            fgd.Position = pos;
                            fgd.CreatedById = UserId;
                            fgd.CreatedByName = "Admin";
                            fgd.CreatedOn = DateTime.UtcNow;
                            fgd.FieldType = pi.PropertyType.Name.ToLower();
                            if (fgd.FieldType == "string")
                                fgd.Width = 150;
                            else if (fgd.FieldType == "datetime")
                                fgd.Width = 100;
                            else if (fgd.FieldType.StartsWith("bool"))
                            {
                                fgd.FieldFormat = "Yes | No";
                                fgd.Width = 75;
                            }
                            else
                                fgd.Width = 100;

                            
                            _unitOfWork.Repository<FormGridDetail>().Add(fgd);

                            pos += 10;
                        }
                    }
                    await _unitOfWork.Complete();
                }
            }

            IReadOnlyList<FormGridDetail> fgds = await _unitOfWork.Repository<FormGridDetail>()
                    .ListAsync(new BaseSpecification<FormGridDetail>(x => x.FormGridHeaderId == fgh.Id));

            return fgds.OrderBy(x => x.Position).ToList();
        }
        private static string GetFieldFormatFromType(Type type)
        {
            if (type == typeof(int) || type == typeof(int?))
                return "Number";
            if (type == typeof(decimal) || type == typeof(float) || type == typeof(double))
                return "Decimal";
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return "Date";
            if (type == typeof(bool) || type == typeof(bool?))
                return "Checkbox";
            return "Text"; // Default fallback
        }


    }
}
