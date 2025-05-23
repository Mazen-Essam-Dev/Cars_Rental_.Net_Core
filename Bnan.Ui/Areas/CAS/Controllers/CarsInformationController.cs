﻿using AutoMapper;
using Bnan.Core.Extensions;
using Bnan.Core.Interfaces;
using Bnan.Core.Models;
using Bnan.Inferastructure.Extensions;
using Bnan.Ui.Areas.Base.Controllers;
using Bnan.Ui.ViewModels.CAS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using NToastNotify;
using System.Diagnostics;
using System.Globalization;

namespace Bnan.Ui.Areas.CAS.Controllers
{
    [Area("CAS")]
    [Authorize(Roles = "CAS")]
    public class CarsInformationController : BaseController
    {
        private readonly IUserLoginsService _userLoginsService;
        private readonly ICarInformation _carInformation;
        private readonly IStringLocalizer<CarsInformationController> _localizer;
        private readonly IToastNotification _toastNotification;
        private readonly IAdminstritiveProcedures _adminstritiveProcedures;
        private readonly IDocumentsMaintainanceCar _documentsMaintainanceCar;

        public CarsInformationController(UserManager<CrMasUserInformation> userManager, IUnitOfWork unitOfWork, IMapper mapper, ICarInformation carInformation, IUserLoginsService userLoginsService, IToastNotification toastNotification, IAdminstritiveProcedures adminstritiveProcedures, IStringLocalizer<CarsInformationController> localizer, IDocumentsMaintainanceCar documentsMaintainanceCar) : base(userManager, unitOfWork, mapper)
        {
            _carInformation = carInformation;
            _userLoginsService = userLoginsService;
            _toastNotification = toastNotification;
            _adminstritiveProcedures = adminstritiveProcedures;
            _localizer = localizer;
            _documentsMaintainanceCar = documentsMaintainanceCar;
        }

        public async Task<IActionResult> CarsInformation()
        {
            var sw = Stopwatch.StartNew();
            var sw2 = Stopwatch.StartNew();
            try
            {

                //save Tracing
                var (mainTask, subTask, system, currentUser) = await SetTrace("202", "2202001", "2");

                await _userLoginsService.SaveTracing(currentUser.CrMasUserInformationCode, "عرض بيانات", "View Informations", mainTask.CrMasSysMainTasksCode,
                subTask.CrMasSysSubTasksCode, mainTask.CrMasSysMainTasksArName, subTask.CrMasSysSubTasksArName, mainTask.CrMasSysMainTasksEnName,
                subTask.CrMasSysSubTasksEnName, system.CrMasSysSystemCode, system.CrMasSysSystemArName, system.CrMasSysSystemEnName);


                //sidebar Active
                ViewBag.id = "#sidebarcars";
                ViewBag.no = "0";
                var userLogin = await _userManager.GetUserAsync(User);
                var lessorCode = userLogin.CrMasUserInformationLessor;
                var titles = await setTitle("202", "2202001", "2");
                await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "", "", titles[3]);
                var cars = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode && (x.CrCasCarInformationStatus != Status.Deleted &&
                                                                                     x.CrCasCarInformationStatus != Status.Sold
                                                                                     ), new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation", "CrCasCarInformationCategoryNavigation", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });
                //var cars = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationStatus == Status.Active,
                //            new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation", "CrCasCarInformationCategoryNavigation",
                //                "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" })?.Select(x => new Car_Spasific_Info {

                //    CrCasCarInformationSerailNo = x.CrCasCarInformationSerailNo,
                //    CrCasCarInformationStatus = x.CrCasCarInformationStatus,
                //    CrCasCarInformationForSaleStatus = x.CrCasCarInformationForSaleStatus,
                //    CrCasCarInformationBranchStatus = x.CrCasCarInformationBranchStatus,
                //    CrCasCarInformationOwnerStatus = x.CrCasCarInformationOwnerStatus,
                //    CrCasCarInformationPriceStatus = x.CrCasCarInformationPriceStatus,
                //    CrCasBranchInformationArShortName = x.CrCasCarInformation1?.CrCasBranchInformationArShortName,
                //    CrCasBranchInformationEnShortName = x.CrCasCarInformation1?.CrCasBranchInformationEnShortName,
                //    CrCasCarInformationConcatenateArName = x.CrCasCarInformationConcatenateArName,
                //    CrCasCarInformationConcatenateEnName = x.CrCasCarInformationConcatenateEnName,
                //    CrCasCarInformationCurrentMeter =  x.CrCasCarInformationCurrentMeter,
                //    CrMasSupCarRegistrationArName = x.CrCasCarInformationRegistrationNavigation?.CrMasSupCarRegistrationArName,
                //    CrMasSupCarRegistrationEnName = x.CrCasCarInformationRegistrationNavigation?.CrMasSupCarRegistrationEnName,
                //    CrMasSupCarFuelArName = x.CrCasCarInformationFuelNavigation?.CrMasSupCarFuelArName,
                //    CrMasSupCarFuelEnName = x.CrCasCarInformationFuelNavigation?.CrMasSupCarFuelEnName,
                //    CrMasSupCarCvtArName = x.CrCasCarInformationCvtNavigation?.CrMasSupCarCvtArName,
                //    CrMasSupCarCvtEnName = x.CrCasCarInformationCvtNavigation?.CrMasSupCarCvtEnName
                // }).ToList();

                sw.Stop();
                sw2 = Stopwatch.StartNew();
                var AdvatagesAll = _unitOfWork.CrCasCarAdvantage.FindAll(x => x.CrCasCarAdvantagesLessor == lessorCode).DistinctBy(x => x.CrCasCarAdvantagesSerialNo).ToList();
                CarsInformation_indexVM carsInformation_IndexVM = new CarsInformation_indexVM();
                carsInformation_IndexVM.AllCars = cars;
                carsInformation_IndexVM.AdvantagesAll = AdvatagesAll;

                return View(carsInformation_IndexVM);
            }
            finally
            {
                sw2.Stop();
                Console.WriteLine($"Took {sw.ElapsedMilliseconds} MilliSeconds");
                Console.WriteLine($"Took {sw2.ElapsedMilliseconds} MilliSeconds");
                //Dispose(); 
            }
        }
        [HttpGet]
        public async Task<PartialViewResult> GetCarsByStatus(string status)
        {
            var sw = Stopwatch.StartNew();
            try
            {


                var userLogin = await _userManager.GetUserAsync(User);
                var lessorCode = userLogin.CrMasUserInformationLessor;

                if (userLogin != null)
                {
                    if (!string.IsNullOrEmpty(status))
                    {
                        //var carsAll = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode, new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation", "CrCasCarInformationCategoryNavigation", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });
                        var AdvatagesAll = _unitOfWork.CrCasCarAdvantage.FindAll(x => x.CrCasCarAdvantagesLessor == lessorCode).DistinctBy(x => x.CrCasCarAdvantagesSerialNo).ToList();
                        List<CrCasCarInformation> Cars = new List<CrCasCarInformation>();
                        CarsInformation_indexVM carsInformation_IndexVM = new CarsInformation_indexVM();

                        sw.Stop();
                        if (status == Status.All)
                        {
                            //Cars = carsAll.Where(x => (x.CrCasCarInformationStatus != Status.Deleted &&
                            //                                                         x.CrCasCarInformationStatus != Status.Sold
                            //                                                         ));
                            var carsAll = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode && (x.CrCasCarInformationStatus != Status.Deleted &&
                                                                                     x.CrCasCarInformationStatus != Status.Sold
                                                                                     ), new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation", "CrCasCarInformationCategoryNavigation", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });

                            carsInformation_IndexVM.AllCars = carsAll;
                            carsInformation_IndexVM.AdvantagesAll = AdvatagesAll;
                            return PartialView("_DataTableCars", carsInformation_IndexVM);
                        }
                        else if (status == "Available")
                        {
                            //Cars = carsAll.Where(x => x.CrCasCarInformationStatus == Status.Active &&
                            //                                                        x.CrCasCarInformationPriceStatus == true && x.CrCasCarInformationBranchStatus == Status.Active &&
                            //                                                        x.CrCasCarInformationOwnerStatus == Status.Active &&
                            //                                                       (x.CrCasCarInformationForSaleStatus == Status.Active || x.CrCasCarInformationForSaleStatus == Status.RendAndForSale));
                            var carsAll2 = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode && x.CrCasCarInformationStatus == Status.Active &&
                                                                                    x.CrCasCarInformationPriceStatus == true && x.CrCasCarInformationBranchStatus == Status.Active &&
                                                                                    x.CrCasCarInformationOwnerStatus == Status.Active &&
                                                                                   (x.CrCasCarInformationForSaleStatus == Status.Active || x.CrCasCarInformationForSaleStatus == Status.RendAndForSale), new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation", "CrCasCarInformationCategoryNavigation", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });

                            carsInformation_IndexVM.AllCars = carsAll2;
                            carsInformation_IndexVM.AdvantagesAll = AdvatagesAll;
                            return PartialView("_DataTableCars", carsInformation_IndexVM);
                        }
                        else if (status == "NotAv")
                        {
                            //Cars = carsAll.Where(x => x.CrCasCarInformationStatus == Status.Rented || x.CrCasCarInformationStatus == Status.Hold ||
                            //                                                        x.CrCasCarInformationStatus == Status.Maintaince || x.CrCasCarInformationPriceStatus == false ||
                            //                                                        x.CrCasCarInformationBranchStatus != Status.Active || x.CrCasCarInformationOwnerStatus != Status.Active ||
                            //                                                       (x.CrCasCarInformationStatus == Status.Active && x.CrCasCarInformationForSaleStatus == Status.ForSale));
                            var carsAll3 = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode && (x.CrCasCarInformationStatus == Status.Hold ||
                                                                                    x.CrCasCarInformationStatus == Status.Maintaince || x.CrCasCarInformationPriceStatus == false ||
                                                                                    x.CrCasCarInformationBranchStatus != Status.Active || x.CrCasCarInformationOwnerStatus != Status.Active ||
                                                                                   (x.CrCasCarInformationStatus == Status.Active && x.CrCasCarInformationForSaleStatus == Status.ForSale)), new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation", "CrCasCarInformationCategoryNavigation", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });
                            carsInformation_IndexVM.AllCars = carsAll3;
                            carsInformation_IndexVM.AdvantagesAll = AdvatagesAll;
                            return PartialView("_DataTableCars", carsInformation_IndexVM);
                        }
                        //Cars = carsAll.Where(x => x.CrCasCarInformationStatus == status);
                        var carsAll4 = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode && x.CrCasCarInformationStatus == status, new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation", "CrCasCarInformationCategoryNavigation", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });
                        carsInformation_IndexVM.AllCars = carsAll4;
                        carsInformation_IndexVM.AdvantagesAll = AdvatagesAll;
                        return PartialView("_DataTableCars", carsInformation_IndexVM);
                    }
                }
                return PartialView();
            }
            finally
            {

                Console.WriteLine($"Took {sw.ElapsedMilliseconds} MilliSeconds");
                //Dispose(); 
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCar()
        {
            //sidebar Active
            ViewBag.id = "#sidebarcars";
            ViewBag.no = "0";
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202001", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "اضافة", "Add", titles[3]);
            var CarsDistribution = _unitOfWork.CrMasSupCarDistribution.FindAll(x => x.CrMasSupCarDistributionStatus == Status.Active).ToList();
            ViewData["CarsDistribtionAr"] = CarsDistribution.Select(x => new SelectListItem { Value = x.CrMasSupCarDistributionCode.ToString(), Text = x.CrMasSupCarDistributionConcatenateArName }).ToList();
            ViewData["CarsDistribtionEn"] = CarsDistribution.Select(x => new SelectListItem { Value = x.CrMasSupCarDistributionCode.ToString(), Text = x.CrMasSupCarDistributionConcatenateEnName }).ToList();
            var RegistrationType = _unitOfWork.CrMasSupCarRegistration.FindAll(x => x.CrMasSupCarRegistrationStatus == Status.Active).ToList();
            ViewData["RegistrationTypeAr"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationArName }).ToList();
            ViewData["RegistrationTypeEn"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationEnName }).ToList();
            var FuelType = _unitOfWork.CrMasSupCarFuel.FindAll(x => x.CrMasSupCarFuelStatus == Status.Active).ToList();
            ViewData["FuelTypeAr"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelArName }).ToList();
            ViewData["FuelTypeEn"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelEnName }).ToList();
            var CVTtype = _unitOfWork.CrMasSupCarCvt.FindAll(x => x.CrMasSupCarCvtStatus == Status.Active).ToList();
            ViewData["CVTtypeAr"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtArName }).ToList();
            ViewData["CVTtypeEn"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtEnName }).ToList();
            var Colors = _unitOfWork.CrMasSupCarColor.FindAll(x => x.CrMasSupCarColorStatus == Status.Active).ToList();
            ViewData["ColorsAr"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorArName }).ToList();
            ViewData["ColorsEn"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorEnName }).ToList();
            ViewBag.Advantages = _unitOfWork.CrMasSupCarAdvantage.FindAll(x => x.CrMasSupCarAdvantagesStatus == Status.Active).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCar(CarsInforamtionVM carsInforamtionVM, List<string> CheckboxAdvantagesWithData, string firstChar, string secondChar, string thirdChar, string BoardNumber)
        {
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            //var CarsDistribution = _unitOfWork.CrMasSupCarDistribution.FindAll(x => x.CrMasSupCarDistributionStatus == Status.Active).ToList();

            // Before Add Must check if the car has price or not

            var serialNoExist = _unitOfWork.CrCasCarInformation.Find(x => x.CrCasCarInformationSerailNo == carsInforamtionVM.CrCasCarInformationSerailNo);
            var SturctureNoExist = _unitOfWork.CrCasCarInformation.Find(x => x.CrCasCarInformationStructureNo == carsInforamtionVM.CrCasCarInformationStructureNo);
            var PlateNoExist = _unitOfWork.CrCasCarInformation.Find(x => x.CrCasCarInformationPlateArNo == carsInforamtionVM.CrCasCarInformationPlateArNo ||
                                                                         x.CrCasCarInformationPlateEnNo == carsInforamtionVM.CrCasCarInformationPlateEnNo);

            if (serialNoExist != null) ModelState.AddModelError("CrCasCarInformationSerailNo", _localizer["IsExists"]);
            if (SturctureNoExist != null) ModelState.AddModelError("CrCasCarInformationStructureNo", _localizer["IsExists"]);
            if (PlateNoExist != null) ModelState.AddModelError("BoardNumber", _localizer["IsExists"]);

            if (!string.IsNullOrEmpty(carsInforamtionVM.CrCasCarInformationDistribution))
            {
                var Distriction = _unitOfWork.CrMasSupCarDistribution.Find(x => x.CrMasSupCarDistributionConcatenateArName == carsInforamtionVM.CrCasCarInformationDistribution ||
                                                                          x.CrMasSupCarDistributionConcatenateEnName == carsInforamtionVM.CrCasCarInformationDistribution);
                if (Distriction == null)
                {
                    ModelState.AddModelError("CrMasSupCarDistributionConcatenateName", _localizer["RessureFromNameCar"]);
                }
                //else carsInforamtionVM.CrCasCarInformationDistribution = Distriction.CrMasSupCarDistributionCode;


                if (ModelState.IsValid)
                {
                    // get plate Number 
                    var firstEnChar = firstChar[0];
                    var secondEnChar = secondChar[0];
                    var ThirdEnChar = thirdChar[0];
                    string firstArChar; if (firstChar[1].ToString() == "ه") firstArChar = "هـ"; else firstArChar = firstChar[1].ToString();
                    string secondArChar; if (secondChar[1].ToString() == "ه") secondArChar = "هـ"; else secondArChar = secondChar[1].ToString();
                    string thirdArChar; if (thirdChar[1].ToString() == "ه") thirdArChar = "هـ"; else thirdArChar = thirdChar[1].ToString();
                    carsInforamtionVM.CrCasCarInformationPlateArNo = $"{firstArChar} {secondArChar} {thirdArChar} {BoardNumber}";
                    carsInforamtionVM.CrCasCarInformationPlateEnNo = $"{firstEnChar} {secondEnChar} {ThirdEnChar} {BoardNumber}";
                    carsInforamtionVM.CrCasCarInformationDistribution = Distriction.CrMasSupCarDistributionCode;
                    var model = _mapper.Map<CrCasCarInformation>(carsInforamtionVM);
                    model.CrCasCarInformationLessor = lessorCode;
                    var carRes = await _carInformation.AddCarInformation(model);
                    var docRes = await _documentsMaintainanceCar.AddDocumentCar(model.CrCasCarInformationSerailNo, lessorCode, "100", (int)model.CrCasCarInformationCurrentMeter);
                    var mainRes = await _documentsMaintainanceCar.AddMaintainaceCar(model.CrCasCarInformationSerailNo, lessorCode, "100", (int)model.CrCasCarInformationCurrentMeter);

                    List<CheckBoxAdvantagesData> checkboxDataList = new List<CheckBoxAdvantagesData>();
                    // Deserialize and filter the checkbox data
                    foreach (var item in CheckboxAdvantagesWithData)
                    {
                        List<CheckBoxAdvantagesData> deserializedData = JsonConvert.DeserializeObject<List<CheckBoxAdvantagesData>>(item);
                        checkboxDataList.AddRange(deserializedData);
                    }
                    foreach (var item in checkboxDataList)
                    {
                        var id = item.Id;
                        var value = item.Value;

                        if (value.ToLower() == "true")
                        {
                            await _carInformation.AddAdvantagesToCar(model.CrCasCarInformationSerailNo, id, model.CrCasCarInformationLessor, model.CrCasCarInformationDistribution, Status.Active);
                        }
                        else
                        {
                            await _carInformation.AddAdvantagesToCar(model.CrCasCarInformationSerailNo, id, model.CrCasCarInformationLessor, model.CrCasCarInformationDistribution, Status.Deleted);
                        }
                    }
                    if (carRes == true && docRes == true && mainRes == true)
                    {
                        await _unitOfWork.CompleteAsync();
                        // SaveTracing
                        var (mainTask, subTask, system, currentUserr) = await SetTrace("202", "2202001", "2");
                        await _userLoginsService.SaveTracing(currentUserr.CrMasUserInformationCode, "اضافة", "Add", mainTask.CrMasSysMainTasksCode,
                        subTask.CrMasSysSubTasksCode, mainTask.CrMasSysMainTasksArName, subTask.CrMasSysSubTasksArName, mainTask.CrMasSysMainTasksEnName,
                        subTask.CrMasSysSubTasksEnName, system.CrMasSysSystemCode, system.CrMasSysSystemArName, system.CrMasSysSystemEnName);
                        // Save Adminstrive Procedures
                        await _adminstritiveProcedures.SaveAdminstritive(currentUserr.CrMasUserInformationCode, "1", "211", "20", currentUserr.CrMasUserInformationLessor, "100",
                            model.CrCasCarInformationSerailNo, null, null, null, null, null, null, null, null, "اضافة", "Insert", "I", null);
                        _toastNotification.AddSuccessToastMessage(_localizer["ToastSave"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                        return RedirectToAction("CarsInformation", "CarsInformation");
                    }
                }
            }
            var RegistrationType = _unitOfWork.CrMasSupCarRegistration.FindAll(x => x.CrMasSupCarRegistrationStatus == Status.Active).ToList();
            ViewData["RegistrationTypeAr"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationArName }).ToList();
            ViewData["RegistrationTypeEn"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationEnName }).ToList();
            var FuelType = _unitOfWork.CrMasSupCarFuel.FindAll(x => x.CrMasSupCarFuelStatus == Status.Active).ToList();
            ViewData["FuelTypeAr"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelArName }).ToList();
            ViewData["FuelTypeEn"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelEnName }).ToList();
            var CVTtype = _unitOfWork.CrMasSupCarCvt.FindAll(x => x.CrMasSupCarCvtStatus == Status.Active).ToList();
            ViewData["CVTtypeAr"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtArName }).ToList();
            ViewData["CVTtypeEn"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtEnName }).ToList();
            var Colors = _unitOfWork.CrMasSupCarColor.FindAll(x => x.CrMasSupCarColorStatus == Status.Active).ToList();
            ViewData["ColorsAr"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorArName }).ToList();
            ViewData["ColorsEn"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorEnName }).ToList();
            ViewBag.Advantages = _unitOfWork.CrMasSupCarAdvantage.FindAll(x => x.CrMasSupCarAdvantagesStatus == Status.Active).ToList();
            var titles = await setTitle("202", "2202001", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "اضافة", "Add", titles[3]);
            return View(carsInforamtionVM);
        }
        [HttpGet]
        public JsonResult GetCarsName()
        {
            var carsInformation = _unitOfWork.CrMasSupCarDistribution.GetAll().OrderByDescending(x => x.CrMasSupCarDistributionCount);

            if (CultureInfo.CurrentCulture.Name == "ar-EG")
            {
                var carArrayAr = carsInformation.Select(c => new { text = c.CrMasSupCarDistributionConcatenateArName, value = c.CrMasSupCarDistributionCode });
                return Json(carArrayAr);
            }
            var carArrayEn = carsInformation.Select(c => new { text = c.CrMasSupCarDistributionConcatenateEnName, value = c.CrMasSupCarDistributionCode });
            return Json(carArrayEn);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string serialNumber)
        {
            //sidebar Active
            ViewBag.id = "#sidebarcars";
            ViewBag.no = "0";
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202001", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "تعديل", "Edit", titles[3]);

            var RegistrationType = _unitOfWork.CrMasSupCarRegistration.FindAll(x => x.CrMasSupCarRegistrationStatus == Status.Active).ToList();
            ViewData["RegistrationTypeAr"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationArName }).ToList();
            ViewData["RegistrationTypeEn"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationEnName }).ToList();
            var FuelType = _unitOfWork.CrMasSupCarFuel.FindAll(x => x.CrMasSupCarFuelStatus == Status.Active).ToList();
            ViewData["FuelTypeAr"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelArName }).ToList();
            ViewData["FuelTypeEn"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelEnName }).ToList();
            var CVTtype = _unitOfWork.CrMasSupCarCvt.FindAll(x => x.CrMasSupCarCvtStatus == Status.Active).ToList();
            ViewData["CVTtypeAr"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtArName }).ToList();
            ViewData["CVTtypeEn"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtEnName }).ToList();
            var Colors = _unitOfWork.CrMasSupCarColor.FindAll(x => x.CrMasSupCarColorStatus == Status.Active).ToList();
            ViewData["ColorsAr"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorArName }).ToList();
            ViewData["ColorsEn"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorEnName }).ToList();
            var car = _unitOfWork.CrCasCarInformation.Find(x => x.CrCasCarInformationSerailNo == serialNumber, new[] { "CrCasCarAdvantages" });
            ViewBag.MainColorAr = _unitOfWork.CrMasSupCarColor.Find(x => x.CrMasSupCarColorCode == car.CrCasCarInformationMainColor).CrMasSupCarColorArName;
            ViewBag.MainColorEn = _unitOfWork.CrMasSupCarColor.Find(x => x.CrMasSupCarColorCode == car.CrCasCarInformationMainColor).CrMasSupCarColorEnName;
            ViewBag.JoinedDate = car.CrCasCarInformationJoinedFleetDate?.ToString("dd/MM/yyyy");
            ViewBag.Advantages = _unitOfWork.CrMasSupCarAdvantage.FindAll(x => x.CrMasSupCarAdvantagesStatus == Status.Active).ToList();
            ViewBag.CurrentMeter = car.CrCasCarInformationCurrentMeter?.ToString("N0");
            if (car != null)
            {
                var carInforamtionVM = _mapper.Map<CarsInforamtionVM>(car);
                return View(carInforamtionVM);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CarsInforamtionVM carsInforamtionVM, List<string> CheckboxAdvantagesWithData)
        {
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;

            var car = _unitOfWork.CrCasCarInformation.Find(x => x.CrCasCarInformationSerailNo == carsInforamtionVM.CrCasCarInformationSerailNo, new[] { "CrCasCarAdvantages" });
            var RegistrationType = _unitOfWork.CrMasSupCarRegistration.FindAll(x => x.CrMasSupCarRegistrationStatus == Status.Active).ToList();
            ViewData["RegistrationTypeAr"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationArName }).ToList();
            ViewData["RegistrationTypeEn"] = RegistrationType.Select(x => new SelectListItem { Value = x.CrMasSupCarRegistrationCode.ToString(), Text = x.CrMasSupCarRegistrationEnName }).ToList();
            var FuelType = _unitOfWork.CrMasSupCarFuel.FindAll(x => x.CrMasSupCarFuelStatus == Status.Active).ToList();
            ViewData["FuelTypeAr"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelArName }).ToList();
            ViewData["FuelTypeEn"] = FuelType.Select(x => new SelectListItem { Value = x.CrMasSupCarFuelCode.ToString(), Text = x.CrMasSupCarFuelEnName }).ToList();
            var CVTtype = _unitOfWork.CrMasSupCarCvt.FindAll(x => x.CrMasSupCarCvtStatus == Status.Active).ToList();
            ViewData["CVTtypeAr"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtArName }).ToList();
            ViewData["CVTtypeEn"] = CVTtype.Select(x => new SelectListItem { Value = x.CrMasSupCarCvtCode.ToString(), Text = x.CrMasSupCarCvtEnName }).ToList();
            var Colors = _unitOfWork.CrMasSupCarColor.FindAll(x => x.CrMasSupCarColorStatus == Status.Active).ToList();
            ViewData["ColorsAr"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorArName }).ToList();
            ViewData["ColorsEn"] = Colors.Select(x => new SelectListItem { Value = x.CrMasSupCarColorCode.ToString(), Text = x.CrMasSupCarColorEnName }).ToList();
            ViewBag.MainColorAr = _unitOfWork.CrMasSupCarColor.Find(x => x.CrMasSupCarColorCode == car.CrCasCarInformationMainColor).CrMasSupCarColorArName;
            ViewBag.MainColorEn = _unitOfWork.CrMasSupCarColor.Find(x => x.CrMasSupCarColorCode == car.CrCasCarInformationMainColor).CrMasSupCarColorEnName;
            ViewBag.JoinedDate = car.CrCasCarInformationJoinedFleetDate?.ToString("dd/MM/yyyy");

            if (car != null)
            {
                if (ModelState.IsValid)
                {
                    var carInforamtion = _mapper.Map<CrCasCarInformation>(carsInforamtionVM);
                    if (await _carInformation.UpdateCarInformation(carInforamtion))
                    {
                        if (CheckboxAdvantagesWithData.Count() > 0)
                        {
                            List<CheckBoxAdvantagesData> checkboxDataList = new List<CheckBoxAdvantagesData>();
                            foreach (var item in CheckboxAdvantagesWithData)
                            {
                                List<CheckBoxAdvantagesData> deserializedData = JsonConvert.DeserializeObject<List<CheckBoxAdvantagesData>>(item);
                                checkboxDataList.AddRange(deserializedData);
                            }
                            foreach (var item in checkboxDataList)
                            {
                                var id = item.Id;
                                var value = item.Value;

                                if (value.ToLower() == "true")
                                {
                                    if (!await _carInformation.UpdateAdvantagesToCar(car.CrCasCarInformationSerailNo, id, car.CrCasCarInformationLessor, Status.Active))
                                    {
                                        _toastNotification.AddErrorToastMessage(_localizer["ToastFailed"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                                        return View(carsInforamtionVM);
                                    }
                                }
                                else
                                {
                                    if (!await _carInformation.UpdateAdvantagesToCar(carInforamtion.CrCasCarInformationSerailNo, id, car.CrCasCarInformationLessor, Status.Deleted))
                                    {
                                        _toastNotification.AddErrorToastMessage(_localizer["ToastFailed"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                                        return View(carsInforamtionVM);
                                    }
                                }
                            }
                        }

                        await _unitOfWork.CompleteAsync();
                        // SaveTracing
                        var (mainTask, subTask, system, currentUserr) = await SetTrace("202", "2202001", "2");
                        await _userLoginsService.SaveTracing(currentUserr.CrMasUserInformationCode, "تعديل", "Edit", mainTask.CrMasSysMainTasksCode,
                        subTask.CrMasSysSubTasksCode, mainTask.CrMasSysMainTasksArName, subTask.CrMasSysSubTasksArName, mainTask.CrMasSysMainTasksEnName,
                        subTask.CrMasSysSubTasksEnName, system.CrMasSysSystemCode, system.CrMasSysSystemArName, system.CrMasSysSystemEnName);
                        // Save Adminstrive Procedures
                        await _adminstritiveProcedures.SaveAdminstritive(currentUserr.CrMasUserInformationCode, "1", "211", "20", currentUserr.CrMasUserInformationLessor, "100",
                            car.CrCasCarInformationSerailNo, null, null, null, null, null, null, null, null, "تعديل", "Edit", "U", null);
                        _toastNotification.AddSuccessToastMessage(_localizer["ToastEdit"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                        return RedirectToAction("CarsInformation", "CarsInformation");
                    }
                    else
                    {
                        _toastNotification.AddErrorToastMessage(_localizer["ToastFailed"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                        return View(carsInforamtionVM);
                    }

                }
            }


            return View(carsInforamtionVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditStatus(string code, string status)
        {
            string sAr = "";
            string sEn = "";
            var userLogin = await _userManager.GetUserAsync(User);
            var car = _unitOfWork.CrCasCarInformation.Find(x => x.CrCasCarInformationSerailNo == code);
            if (car != null)
            {
                var docs = _unitOfWork.CrCasCarDocumentsMaintenance.FindAll(l => l.CrCasCarDocumentsMaintenanceLessor == car.CrCasCarInformationLessor && l.CrCasCarDocumentsMaintenanceSerailNo == car.CrCasCarInformationSerailNo);
                if (status == Status.Hold)
                {
                    sAr = "ايقاف ";
                    sEn = "Hold ";
                    car.CrCasCarInformationStatus = Status.Hold;
                    foreach (var doc in docs) doc.CrCasCarDocumentsMaintenanceCarStatus = Status.Hold;
                }
                else if (status == Status.Deleted)
                {
                    sAr = "حذف";
                    sEn = "Remove";
                    car.CrCasCarInformationStatus = Status.Deleted;
                    foreach (var doc in docs) doc.CrCasCarDocumentsMaintenanceCarStatus = Status.Deleted;

                }
                else if (status == Status.Active)
                {
                    sAr = "استرجاع";
                    sEn = "Retrive";
                    car.CrCasCarInformationStatus = Status.Active;
                    foreach (var doc in docs) doc.CrCasCarDocumentsMaintenanceCarStatus = Status.Active;
                }
                else if (status == Status.ForSale)
                {
                    sAr = "عرض للبيع";
                    sEn = "For Sale";
                    car.CrCasCarInformationStatus = Status.ForSale;
                    foreach (var doc in docs) doc.CrCasCarDocumentsMaintenanceCarStatus = Status.ForSale;

                }
                await _unitOfWork.CompleteAsync();
                // SaveTracing
                var (mainTask, subTask, system, currentUserr) = await SetTrace("202", "2202001", "2");
                await _userLoginsService.SaveTracing(currentUserr.CrMasUserInformationCode, "عرض للبيع", "For Sale", mainTask.CrMasSysMainTasksCode,
                subTask.CrMasSysSubTasksCode, mainTask.CrMasSysMainTasksArName, subTask.CrMasSysSubTasksArName, mainTask.CrMasSysMainTasksEnName,
                subTask.CrMasSysSubTasksEnName, system.CrMasSysSystemCode, system.CrMasSysSystemArName, system.CrMasSysSystemEnName);
                // Save Adminstrive Procedures
                await _adminstritiveProcedures.SaveAdminstritive(currentUserr.CrMasUserInformationCode, "1", "211", "20", currentUserr.CrMasUserInformationLessor, "100",
                    car.CrCasCarInformationSerailNo, null, null, null, null, null, null, null, null, sAr, sEn, "U", null);
                return RedirectToAction("CarsInformation", "CarsInformation");
            }
            _toastNotification.AddErrorToastMessage(_localizer["ToastFailed"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
            return RedirectToAction("CarsInformation", "CarsInformation");
        }
        public async Task<IActionResult> TransfersCarsBranch()
        {
            //sidebar Active
            ViewBag.id = "#sidebarcars";
            ViewBag.no = "4";
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202005", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "", "", titles[3]);
            var cars = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode && x.CrCasCarInformationStatus == Status.Active &&
                                                                    x.CrCasCarInformationBranchStatus == Status.Active
                                                                   , new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation",
                                                                                                                               "CrCasCarInformationCategoryNavigation", "CrCasCarInformation2", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });
            return View(cars);
        }
        [HttpGet]
        public async Task<IActionResult> TransfersCarToBranch(string serialNumber)
        {
            //sidebar Active
            ViewBag.id = "#sidebarcars";
            ViewBag.no = "4";
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202005", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "تعديل", "Edit", titles[3]);
            var car = await _unitOfWork.CrCasCarInformation.FindAsync(x => x.CrCasCarInformationSerailNo == serialNumber && x.CrCasCarInformationLessor == lessorCode, new[] {"CrCasCarInformation1", "CrCasCarInformationDistributionNavigation",
                                                                                                                               "CrCasCarInformationCategoryNavigation", "CrCasCarInformation2"});
            var branches = _unitOfWork.CrCasBranchInformation.FindAll(x => x.CrCasBranchInformationLessor == lessorCode && x.CrCasBranchInformationCode != car.CrCasCarInformationBranch && x.CrCasBranchInformationStatus == Status.Active);
            ViewData["BranchAr"] = branches.Select(x => new SelectListItem { Value = x.CrCasBranchInformationCode.ToString(), Text = x.CrCasBranchInformationArShortName }).ToList();
            ViewData["BranchEn"] = branches.Select(x => new SelectListItem { Value = x.CrCasBranchInformationCode.ToString(), Text = x.CrCasBranchInformationEnShortName }).ToList();
            var carVM = _mapper.Map<CarsInforamtionVM>(car);
            return View(carVM);
        }
        [HttpPost]
        public async Task<IActionResult> TransfersCarToBranch(CarsInforamtionVM model, string NewBranch)
        {
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202005", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "تعديل", "Edit", titles[3]);
            var car = await _unitOfWork.CrCasCarInformation.FindAsync(x => x.CrCasCarInformationSerailNo == model.CrCasCarInformationSerailNo && x.CrCasCarInformationLessor == lessorCode, new[] {"CrCasCarInformation1", "CrCasCarInformationDistributionNavigation",
                                                                                                                               "CrCasCarInformationCategoryNavigation", "CrCasCarInformation2"});
            var oldBranch = car.CrCasCarInformationBranch;
            var postBranch = _unitOfWork.CrCasBranchPost.Find(x => x.CrCasBranchPostLessor == lessorCode && x.CrCasBranchPostBranch == NewBranch);
            var docAndMain = _unitOfWork.CrCasCarDocumentsMaintenance.FindAll(x => x.CrCasCarDocumentsMaintenanceSerailNo == car.CrCasCarInformationSerailNo &&
                                                                                x.CrCasCarDocumentsMaintenanceLessor == lessorCode &&
                                                                                x.CrCasCarDocumentsMaintenanceBranch == oldBranch);

            if (car != null && postBranch != null)
            {
                car.CrCasCarInformationBranch = postBranch.CrCasBranchPostBranch;
                car.CrCasCarInformationCity = postBranch.CrCasBranchPostCity;
                car.CrCasCarInformationRegion = postBranch.CrCasBranchPostRegions;
                car.CrCasCarInformationReasons = model.CrCasCarInformationReasons;
                foreach (var item in docAndMain) item.CrCasCarDocumentsMaintenanceBranch = car.CrCasCarInformationBranch;
                _unitOfWork.CrCasCarInformation.Update(car);
                await _unitOfWork.CompleteAsync();
                // SaveTracing
                var (mainTask, subTask, system, currentUserr) = await SetTrace("202", "2202004", "2");
                await _userLoginsService.SaveTracing(currentUserr.CrMasUserInformationCode, "تعديل", "Edit", mainTask.CrMasSysMainTasksCode,
                subTask.CrMasSysSubTasksCode, mainTask.CrMasSysMainTasksArName, subTask.CrMasSysSubTasksArName, mainTask.CrMasSysMainTasksEnName,
                subTask.CrMasSysSubTasksEnName, system.CrMasSysSystemCode, system.CrMasSysSystemArName, system.CrMasSysSystemEnName);
                // Save Adminstrive Procedures
                await _adminstritiveProcedures.SaveAdminstritive(currentUserr.CrMasUserInformationCode, "1", "215", "20", currentUserr.CrMasUserInformationLessor, "100",
                    car.CrCasCarInformationSerailNo, null, null, null, null, null, null, oldBranch, car.CrCasCarInformationBranch, "تعديل", "Edit", "U", null);
                _toastNotification.AddSuccessToastMessage(_localizer["ToastEdit"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                return RedirectToAction("TransfersCarsBranch", "CarsInformation");
            }

            var branches = _unitOfWork.CrCasBranchInformation.FindAll(x => x.CrCasBranchInformationLessor == lessorCode && x.CrCasBranchInformationCode != car.CrCasCarInformationBranch);
            ViewData["BranchAr"] = branches.Select(x => new SelectListItem { Value = x.CrCasBranchInformationCode.ToString(), Text = x.CrCasBranchInformationArName }).ToList();
            ViewData["BranchEn"] = branches.Select(x => new SelectListItem { Value = x.CrCasBranchInformationCode.ToString(), Text = x.CrCasBranchInformationEnName }).ToList();
            return View(model);
        }

        public async Task<IActionResult> TransfersOwner()
        {
            //sidebar Active
            ViewBag.id = "#sidebarcars";
            ViewBag.no = "5";
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202006", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "", "", titles[3]);
            var cars = _unitOfWork.CrCasCarInformation.FindAll(x => x.CrCasCarInformationLessor == lessorCode && x.CrCasCarInformationStatus == Status.Active &&
                                                                    x.CrCasCarInformationBranchStatus == Status.Active &&
                                                                    x.CrCasCarInformationOwnerStatus == Status.Active, new[] { "CrCasCarInformation1", "CrCasCarInformationDistributionNavigation",
                                                                                                                               "CrCasCarInformationCategoryNavigation", "CrCasCarInformation2", "CrCasCarInformationFuelNavigation", "CrCasCarInformationCvtNavigation", "CrCasCarInformationRegistrationNavigation" });
            return View(cars);
        }

        public async Task<IActionResult> TransfersCarToOwner(string serialNumber)
        {
            //sidebar Active
            ViewBag.id = "#sidebarcars";
            ViewBag.no = "5";
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202006", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "تعديل", "Edit", titles[3]);
            var car = await _unitOfWork.CrCasCarInformation.FindAsync(x => x.CrCasCarInformationSerailNo == serialNumber && x.CrCasCarInformationLessor == lessorCode, new[] {"CrCasCarInformation1", "CrCasCarInformationDistributionNavigation",
                                                                                                                               "CrCasCarInformationCategoryNavigation", "CrCasCarInformation2"});
            var Owners = _unitOfWork.CrCasOwners.FindAll(x => x.CrCasOwnersLessorCode == lessorCode && x.CrCasOwnersCode != car.CrCasCarInformationOwner && x.CrCasOwnersStatus == Status.Active);
            ViewData["OwnersAr"] = Owners.Select(x => new SelectListItem { Value = x.CrCasOwnersCode.ToString(), Text = x.CrCasOwnersArName }).ToList();
            ViewData["OwnersEn"] = Owners.Select(x => new SelectListItem { Value = x.CrCasOwnersCode.ToString(), Text = x.CrCasOwnersEnName }).ToList();
            var carVM = _mapper.Map<CarsInforamtionVM>(car);
            return View(carVM);
        }
        [HttpPost]
        public async Task<IActionResult> TransfersCarToOwner(CarsInforamtionVM model, string NewOwner)
        {
            //sidebar Active
            ViewBag.id = "#sidebarcars";
            ViewBag.no = "5";
            var userLogin = await _userManager.GetUserAsync(User);
            var lessorCode = userLogin.CrMasUserInformationLessor;
            var titles = await setTitle("202", "2202006", "2");
            await ViewData.SetPageTitleAsync(titles[0], titles[1], titles[2], "تعديل", "Edit", titles[3]);
            var car = await _unitOfWork.CrCasCarInformation.FindAsync(x => x.CrCasCarInformationSerailNo == model.CrCasCarInformationSerailNo && x.CrCasCarInformationLessor == lessorCode, new[] {"CrCasCarInformation1", "CrCasCarInformationDistributionNavigation",
                                                                                                                               "CrCasCarInformationCategoryNavigation", "CrCasCarInformation2"});

            var oldOwnerCode = car.CrCasCarInformationOwner;
            var owner = await _unitOfWork.CrCasOwners.FindAsync(x => x.CrCasOwnersCode == NewOwner && x.CrCasOwnersLessorCode == lessorCode);
            if (car != null && owner != null)
            {
                car.CrCasCarInformationOwner = owner.CrCasOwnersCode;
                car.CrCasCarInformationReasons = model.CrCasCarInformationReasons;
                _unitOfWork.CrCasCarInformation.Update(car);
                await _unitOfWork.CompleteAsync();
                // SaveTracing
                var (mainTask, subTask, system, currentUserr) = await SetTrace("202", "2202005", "2");
                await _userLoginsService.SaveTracing(currentUserr.CrMasUserInformationCode, "تعديل", "Edit", mainTask.CrMasSysMainTasksCode,
                subTask.CrMasSysSubTasksCode, mainTask.CrMasSysMainTasksArName, subTask.CrMasSysSubTasksArName, mainTask.CrMasSysMainTasksEnName,
                subTask.CrMasSysSubTasksEnName, system.CrMasSysSystemCode, system.CrMasSysSystemArName, system.CrMasSysSystemEnName);
                // Save Adminstrive Procedures
                await _adminstritiveProcedures.SaveAdminstritive(currentUserr.CrMasUserInformationCode, "1", "216", "20", currentUserr.CrMasUserInformationLessor, "100",
                    car.CrCasCarInformationSerailNo, null, null, null, null, null, null, oldOwnerCode, car.CrCasCarInformationOwner, "تعديل", "Edit", "U", null);
                _toastNotification.AddSuccessToastMessage(_localizer["ToastEdit"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                return RedirectToAction("TransfersOwner", "CarsInformation");
            }

            var Owners = _unitOfWork.CrCasOwners.FindAll(x => x.CrCasOwnersLessorCode == lessorCode && x.CrCasOwnersCode != car.CrCasCarInformationOwner);
            ViewData["OwnersAr"] = Owners.Select(x => new SelectListItem { Value = x.CrCasOwnersCode.ToString(), Text = x.CrCasOwnersArName }).ToList();
            ViewData["OwnersEn"] = Owners.Select(x => new SelectListItem { Value = x.CrCasOwnersCode.ToString(), Text = x.CrCasOwnersEnName }).ToList();
            return View(model);
        }
        public IActionResult SuccesssMessageForCarsInformation()
        {
            _toastNotification.AddSuccessToastMessage(_localizer["ToastEdit"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
            return RedirectToAction("CarsInformation", "CarsInformation");
        }

    }
}
