﻿using AutoMapper;
using Bnan.Core.Extensions;
using Bnan.Core.Interfaces;
using Bnan.Core.Models;
using Bnan.Inferastructure.Extensions;
using Bnan.Ui.Areas.Base.Controllers;
using Bnan.Ui.ViewModels.BS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NToastNotify;

namespace Bnan.Ui.Areas.BS.Controllers
{
    [Area("BS")]
    [Authorize(Roles = "BS")]
    public class EmployeesController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IStringLocalizer<CustodyController> _localizer;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public EmployeesController(UserManager<CrMasUserInformation> userManager, IUnitOfWork unitOfWork, IMapper mapper, IUserService userService, IAuthService authService, IStringLocalizer<CustodyController> localizer, IToastNotification toastNotification, IWebHostEnvironment webHostEnvironment) : base(userManager, unitOfWork, mapper)
        {
            _userService = userService;
            _authService = authService;
            _localizer = localizer;
            _toastNotification = toastNotification;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> MyAccount()
        { //To Set Title 
            var titles = await setTitle("501", "5501005", "5");
            await ViewData.SetPageTitleAsync(titles[0], "", titles[2], "", "", titles[3]);
            var bsLayoutVM = await GetBranchesAndLayout();
            var userLogin = await _userManager.GetUserAsync(User);
            var user = await _userService.GetUserByUserNameAsync(userLogin.CrMasUserInformationCode);
            var CallingKeys = _unitOfWork.CrMasSysCallingKeys.FindAll(x => x.CrMasSysCallingKeysStatus == Status.Active).ToList();
            bsLayoutVM.UserInformation = user;
            bsLayoutVM.CallingKeys = CallingKeys;
            return View(bsLayoutVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BSLayoutVM model, IFormFile Profile_Photo, IFormFile signature_img, string signatureImg1)
        {
            var userLogin = await _userManager.GetUserAsync(User);
            var user = await _userService.GetUserByUserNameAsync(userLogin.CrMasUserInformationCode);
            var LessorCode = user?.CrMasUserInformationLessor;
            var UserModel = model.UserInformation;
            string foldername = $"{"images\\Company"}\\{user?.CrMasUserInformationLessor}\\{"Users"}\\{user?.CrMasUserInformationCode}";

            string filePathImage;
            string filePathSignture;

            var oldPathImage = user.CrMasUserInformationPicture;
            var oldPathSignture = user.CrMasUserInformationSignature;
            if (oldPathImage == "~/images/common/user.jpg") oldPathImage = "";
            if (oldPathSignture == "~/images/common/DefualtUserSignature.png") oldPathSignture = "";

            if (Profile_Photo != null)
            {
                string fileNameImg = "Image_" + DateTime.Now.ToString("yyyyMMddHHmmss"); // اسم مبني على التاريخ والوقت
                filePathImage = await Profile_Photo.SaveImageAsync(_webHostEnvironment, foldername, fileNameImg, ".png", oldPathImage);
            }
            else if (Profile_Photo == null && string.IsNullOrEmpty(user.CrMasUserInformationPicture))
            {
                filePathImage = "~/images/common/user.jpg";
            }
            else
            {
                filePathImage = user.CrMasUserInformationPicture;
            }
            // Signture 
            if (!string.IsNullOrEmpty(signatureImg1))
            {
                filePathSignture = await FileExtensions.SaveSigntureImage(_webHostEnvironment, signatureImg1, user.CrMasUserInformationCode, user.CrMasUserInformationSignature, foldername);
            }
            else
            {
                filePathSignture = user.CrMasUserInformationSignature;
            }

            if (user != null)
            {
                user.CrMasUserInformationDefaultLanguage = UserModel.CrMasUserInformationDefaultLanguage;
                //user.CrMasUserInformationMobileNo = UserModel.CrMasUserInformationMobileNo;
                user.CrMasUserInformationEmail = UserModel.CrMasUserInformationEmail;
                user.CrMasUserInformationRemindMe = UserModel.CrMasUserInformationRemindMe;
                user.CrMasUserInformationExitTimer = UserModel.CrMasUserInformationExitTimer;
                //user.CrMasUserInformationCallingKey = UserModel.CrMasUserInformationCallingKey;
                user.CrMasUserInformationPicture = filePathImage;
                user.CrMasUserInformationSignature = filePathSignture;
                try
                {
                    await _userService.UpdateAsync(user);
                    _toastNotification.AddSuccessToastMessage(_localizer["ToastEdit"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                    return RedirectToAction("Index", "Home", new { area = "BS" });
                }
                catch (Exception)
                {
                    _toastNotification.AddErrorToastMessage(_localizer["ToastFailed"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
                    return RedirectToAction("Index", "Home", new { area = "BS" });
                    throw;
                }

            }

            _toastNotification.AddErrorToastMessage(_localizer["ToastFailed"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
            return RedirectToAction("Index", "Home", new { area = "BS" });
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string Current, string New)
        {
            var userLogin = await _userManager.GetUserAsync(User);
            var user = await _userService.GetUserByUserNameAsync(userLogin.CrMasUserInformationCode);
            // Check current password 
            if (!await _userManager.CheckPasswordAsync(user, Current)) return Json("wrong");
            var result = await _userManager.ChangePasswordAsync(user, Current.Trim(), New.Trim());
            if (result.Succeeded)
            {
                //user.CrMasUserInformationPassWord = New.Trim();
                user.CrMasUserInformationChangePassWordLastDate = DateTime.Now.Date;
                _unitOfWork.Complete();
                return Json("true");
            }
            else return Json("false");
        }
        public IActionResult SuccesssMessageForChangePassword()
        {
            _toastNotification.AddSuccessToastMessage(_localizer["ToastEdit"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
            return RedirectToAction("Index", "Home", new { area = "BS" });
        }
        public IActionResult ErrorMessageForChangePassword()
        {
            _toastNotification.AddErrorToastMessage(_localizer["SomethingWrongPleaseCallAdmin"], new ToastrOptions { PositionClass = _localizer["toastPostion"] });
            return RedirectToAction("Index", "Home", new { area = "BS" });
        }

    }
}
