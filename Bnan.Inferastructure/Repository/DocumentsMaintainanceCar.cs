﻿using Bnan.Core.Extensions;
using Bnan.Core.Interfaces;
using Bnan.Core.Models;

namespace Bnan.Inferastructure.Repository
{
    public class DocumentsMaintainanceCar : IDocumentsMaintainanceCar
    {
        private IUnitOfWork _unitOfWork;

        public DocumentsMaintainanceCar(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> AddDocumentCar(string serialNumber, string lessorCode, string branchCode, int currentMeter)
        {
            var lessorMechanisms = _unitOfWork.CrCasLessorMechanism.FindAll(l => l.CrCasLessorMechanismCode == lessorCode && l.CrCasLessorMechanismProceduresClassification == "12");
            try
            {
                if (lessorMechanisms != null)
                {
                    foreach (var item in lessorMechanisms)
                    {
                        CrCasCarDocumentsMaintenance crCasCarDocumentsMaintenance = new CrCasCarDocumentsMaintenance
                        {
                            CrCasCarDocumentsMaintenanceSerailNo = serialNumber,
                            CrCasCarDocumentsMaintenanceProcedures = item.CrCasLessorMechanismProcedures,
                            CrCasCarDocumentsMaintenanceProceduresClassification = item.CrCasLessorMechanismProceduresClassification,
                            CrCasCarDocumentsMaintenanceIsActivation = item.CrCasLessorMechanismActivate,
                            CrCasCarDocumentsMaintenanceLessor = item.CrCasLessorMechanismCode,
                            CrCasCarDocumentsMaintenanceBranch = branchCode,
                            CrCasCarDocumentsMaintenanceCurrentMeter = currentMeter,
                            CrCasCarDocumentsMaintenanceStatus = "N",
                        };

                        await _unitOfWork.CrCasCarDocumentsMaintenance.AddAsync(crCasCarDocumentsMaintenance);
                    }
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> AddMaintainaceCar(string serialNumber, string lessorCode, string branchCode, int currentMeter)
        {
            var lessorMechanisms = _unitOfWork.CrCasLessorMechanism.FindAll(l => l.CrCasLessorMechanismCode == lessorCode && l.CrCasLessorMechanismProceduresClassification == "13");
            try
            {
                if (lessorMechanisms != null)
                {
                    foreach (var item in lessorMechanisms)
                    {
                        CrCasCarDocumentsMaintenance crCasCarDocumentsMaintenance = new CrCasCarDocumentsMaintenance
                        {
                            CrCasCarDocumentsMaintenanceSerailNo = serialNumber,
                            CrCasCarDocumentsMaintenanceProcedures = item.CrCasLessorMechanismProcedures,
                            CrCasCarDocumentsMaintenanceProceduresClassification = item.CrCasLessorMechanismProceduresClassification,
                            CrCasCarDocumentsMaintenanceIsActivation = item.CrCasLessorMechanismActivate,
                            CrCasCarDocumentsMaintenanceLessor = item.CrCasLessorMechanismCode,
                            CrCasCarDocumentsMaintenanceBranch = branchCode,
                            CrCasCarDocumentsMaintenanceCurrentMeter = currentMeter,
                            CrCasCarDocumentsMaintenanceStatus = "N",
                        };

                        await _unitOfWork.CrCasCarDocumentsMaintenance.AddAsync(crCasCarDocumentsMaintenance);
                    }
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckMaintainceAndDocsCar(string serialNumber, string lessorCode, string classificationCode, string procudureCode)
        {
            var docsAndMaintaince = await _unitOfWork.CrCasCarDocumentsMaintenance.FindAllAsNoTrackingAsync(x => x.CrCasCarDocumentsMaintenanceSerailNo == serialNumber &&
                                                                                                           x.CrCasCarDocumentsMaintenanceLessor == lessorCode &&
                                                                                                           x.CrCasCarDocumentsMaintenanceProceduresClassification == classificationCode && x.CrCasCarDocumentsMaintenanceProcedures != procudureCode);
            var carInfo = await _unitOfWork.CrCasCarInformation.FindAsync(x => x.CrCasCarInformationSerailNo == serialNumber && x.CrCasCarInformationLessor == lessorCode);

            var main = docsAndMaintaince.Any(x => x.CrCasCarDocumentsMaintenanceStatus == Status.Expire || x.CrCasCarDocumentsMaintenanceStatus == Status.Renewed);
            if (!main)
            {
                if (classificationCode == "12") carInfo.CrCasCarInformationDocumentationStatus = true;
                else carInfo.CrCasCarInformationMaintenanceStatus = true;
                if (_unitOfWork.CrCasCarInformation.Update(carInfo) != null) return true;
                else return false;
            }
            return true;
        }

        public async Task<bool> UpdateDocumentCar(CrCasCarDocumentsMaintenance crCasCarDocumentsMaintenance)
        {
            if (crCasCarDocumentsMaintenance != null)
            {
                var CarDocument = await _unitOfWork.CrCasCarDocumentsMaintenance.FindAsync(l => l.CrCasCarDocumentsMaintenanceBranch == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceBranch
                                                                               && l.CrCasCarDocumentsMaintenanceLessor == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceLessor
                                                                               && l.CrCasCarDocumentsMaintenanceProcedures == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceProcedures
                                                                               && l.CrCasCarDocumentsMaintenanceSerailNo == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceSerailNo);
                if (CarDocument != null)
                {
                    var AboutToExpire = _unitOfWork.CrCasLessorMechanism.FindAsync(l => l.CrCasLessorMechanismCode == CarDocument.CrCasCarDocumentsMaintenanceLessor
                                                                               && l.CrCasLessorMechanismProcedures == CarDocument.CrCasCarDocumentsMaintenanceProcedures
                                                                               && l.CrCasLessorMechanismProceduresClassification == CarDocument.CrCasCarDocumentsMaintenanceProceduresClassification).Result.CrCasLessorMechanismDaysAlertAboutExpire;
                    if (crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceStatus == Status.Renewed || crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceStatus == Status.Expire)
                    {
                        CarDocument.CrCasCarDocumentsMaintenanceSerailNo = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceSerailNo;
                        CarDocument.CrCasCarDocumentsMaintenanceStartDate = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceStartDate;
                        CarDocument.CrCasCarDocumentsMaintenanceEndDate = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceEndDate;
                        CarDocument.CrCasCarDocumentsMaintenanceDate = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceDate;
                        CarDocument.CrCasCarDocumentsMaintenanceNo = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceNo;
                        CarDocument.CrCasCarDocumentsMaintenanceImage = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceImage;
                        CarDocument.CrCasCarDocumentsMaintenanceDateAboutToFinish = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceEndDate?.AddDays(-(double)AboutToExpire);
                        CarDocument.CrCasCarDocumentsMaintenanceStatus = Status.Active;
                        CarDocument.CrCasCarDocumentsMaintenanceReasons = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceReasons;
                    }
                    else
                    {
                        CarDocument.CrCasCarDocumentsMaintenanceReasons = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceReasons;
                        CarDocument.CrCasCarDocumentsMaintenanceImage = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceImage;
                    }
                    _unitOfWork.CrCasCarDocumentsMaintenance.Update(CarDocument);
                    return true;
                }
                return false;
            }
            return false;
        }
        public async Task<bool> UpdateMaintainceCar(CrCasCarDocumentsMaintenance crCasCarDocumentsMaintenance)
        {
            if (crCasCarDocumentsMaintenance != null)
            {
                var CarDocument = await _unitOfWork.CrCasCarDocumentsMaintenance.FindAsync(l => l.CrCasCarDocumentsMaintenanceBranch == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceBranch
                                                                               && l.CrCasCarDocumentsMaintenanceLessor == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceLessor
                                                                               && l.CrCasCarDocumentsMaintenanceProcedures == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceProcedures
                                                                               && l.CrCasCarDocumentsMaintenanceSerailNo == crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceSerailNo);
                if (CarDocument != null)
                {
                    var AboutToExpire = _unitOfWork.CrCasLessorMechanism.FindAsync(l => l.CrCasLessorMechanismCode == CarDocument.CrCasCarDocumentsMaintenanceLessor
                                                                               && l.CrCasLessorMechanismProcedures == CarDocument.CrCasCarDocumentsMaintenanceProcedures
                                                                               && l.CrCasLessorMechanismProceduresClassification == CarDocument.CrCasCarDocumentsMaintenanceProceduresClassification)
                                                                               .Result.CrCasLessorMechanismKmAlertAboutExpire;
                    if (crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceStatus == Status.Renewed || crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceStatus == Status.Expire)
                    {
                        CarDocument.CrCasCarDocumentsMaintenanceSerailNo = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceSerailNo;
                        CarDocument.CrCasCarDocumentsMaintenanceStartDate = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceStartDate;
                        CarDocument.CrCasCarDocumentsMaintenanceEndDate = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceEndDate;
                        CarDocument.CrCasCarDocumentsMaintenanceNo = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceNo;
                        CarDocument.CrCasCarDocumentsMaintenanceConsumptionKm = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceConsumptionKm;
                        CarDocument.CrCasCarDocumentsMaintenanceCurrentMeter = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceCurrentMeter;
                        CarDocument.CrCasCarDocumentsMaintenanceKmEndsAt = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceKmEndsAt;
                        CarDocument.CrCasCarDocumentsMaintenanceKmAboutToFinish = (crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceKmEndsAt - (int)AboutToExpire);
                        CarDocument.CrCasCarDocumentsMaintenanceStatus = Status.Active;
                        CarDocument.CrCasCarDocumentsMaintenanceReasons = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceReasons;
                    }
                    else
                    {
                        CarDocument.CrCasCarDocumentsMaintenanceReasons = crCasCarDocumentsMaintenance.CrCasCarDocumentsMaintenanceReasons;
                    }
                    _unitOfWork.CrCasCarDocumentsMaintenance.Update(CarDocument);
                    return true;
                }
                return false;
            }
            return false;
        }

    }
}
