// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.

export const environment = {
  production: false,

  endpointAccountLogin: 'https://localhost:44366/api/Auth/Login',
  endpointAccountLogout: 'https://localhost:44366/api/Auth/Logout',
  endpointAccountUserInfo: 'https://localhost:44366/api/Account/UserInfo',
  endpointAccountForApproval: 'https://localhost:44366/api/Account/AccountForApproval',
  endpointAccountsForApproval: 'https://localhost:44366/api/Account/AccountsForApproval',
  endpointAccountApproveAccount: 'https://localhost:44366/api/Account/ApproveAccount',
  endpointAccountRejectAccount: 'https://localhost:44366/api/Account/RejectAccount',
  endpointAccountGetUsers: 'https://localhost:44366/api/Account/GetUsers',
  endpointAccountGetManagers: 'https://localhost:44366/api/Account/GetManagers',
  endpointAccountGetRoles: 'https://localhost:44366/api/Account/GetRoles', 
  endpointAccountChangeRole: 'https://localhost:44366/api/Account/ChangeRole?userId=',
  endpointAccountChangeManagerBan: 'https://localhost:44366/api/Account/ChangeManagerBan',
  endpointAccountIsUserApproved: 'https://localhost:44366/api/Account/IsUserApproved',
  endpointAccountHubConnection: 'https://localhost:44366/notificationHub',
  endpointAccountRegister: 'https://localhost:44366/api/Account/Register',
  endpointAccountFinish: 'https://localhost:44366/api/Account/FinishAccount',

  endpointAccountGetUserImage: 'https://localhost:44366/api/Account/UserDocumentImage?imageId=',

  endpointBranchOfficeGetBranchOffice: 'https://localhost:44367/api/BranchOffice/GetBranchOffice?id=',
  endpointBranchOfficeGetBranchOffices: 'https://localhost:44367/api/Service/GetBranchOffices?serviceId=',
  endpointBranchOfficeGetVehicle: 'https://localhost:44367/api/BranchOffice/GetVehicleBranchOffices?vehicleId=',
  endpointBranchOfficeDelete: 'https://localhost:44367/api/BranchOffice/DeleteBranchOffice?serviceId=',
  endpointBranchOfficeCreate: 'https://localhost:44367/api/BranchOffice/PostBranchOffice',
  endpointBranchOfficeEdit: 'https://localhost:44367/api/BranchOffice/PutBranchOffice',
  endpointRentVehicleServicesForApprovalCount: 'https://localhost:44367/api/Service/ServicesForApprovalCount',
  endpointRentVehicleHubConnection: 'https://localhost:44367/notificationHub',
  endpointRentVehicleGetVehicleType: 'https://localhost:44367/api/VehicleType/GetVehicleType?vehicleTypeId=',
  endpointRentVehicleGetVehicleTypes: 'https://localhost:44367/api/VehicleType/GetVehicleTypes',
  endpointRentVehicleVehicleTypes: "https://localhost:44367/api/VehicleTypes/",
  endpointRentVehicleTypeCreateVehicleType: 'https://localhost:44367/api/VehicleType/PostVehicleType',
  endpointRentVehicleTypeEditVehicleType: 'https://localhost:44367/api/VehicleType/PutVehicleType',
  endpointRentVehicleGetService: 'https://localhost:44367/api/Service/GetService?serviceId=',
  endpointRentVehicleGetServices: 'https://localhost:44367/api/Service/GetServices',
  endpointRentVehicleGetServicesForApproval: 'https://localhost:44367/api/Service/ServicesForApproval',
  endpointRentVehicleDeleteService: 'https://localhost:44367/api/Service/DeleteService?serviceId=',
  endpointRentVehicleApproveService: 'https://localhost:44367/api/Service/ApproveService',
  endpointRentVehicleRejectService: 'https://localhost:44367/api/Service/RejectService',
  endpointRentVehicleCreateService: 'https://localhost:44367/api/Service/PostService',
  endpointRentVehicleEditService: 'https://localhost:44367/api/Service/PutService',
  endpointRentVehicleGetVehicles: 'https://localhost:44367/api/Vehicle/GetVehicles',
  endpointRentVehicleGetServiceVehicles: 'https://localhost:44367/api/Service/GetVehicles?serviceId=',
  endpointRentVehicleGetVehicle: 'https://localhost:44367/api/Vehicle/GetVehicle?id=',
  endpointRentVehicleGetNumberOfVehicles: 'https://localhost:44367/api/Vehicle/GetNumberOfVehicles',
  endpointRentVehicleCreateVehicle: 'https://localhost:44367/api/Vehicle/PostVehicle',
  endpointRentVehicleEditVehicle: 'https://localhost:44367/api/Vehicle/PutVehicle',
  endpointRentVehicleDeleteVehicle: 'https://localhost:44367/api/Vehicle/DeleteVehicle?id=',
  endpointRentVehicleGetPagedVehicles: 'https://localhost:44367/api/Vehicle/GetPagedVehicles?pageIndex=',
  endpointRentVehicleGetSearchPagedVehicles: 'https://localhost:44367/api/Vehicle/GetSearchPagedVehicles?pageIndex=',
  endpointRentVehicleSearchNumberOfVehicles: 'https://localhost:44367/api/Vehicle/SearchNumberOfVehicles?vehicleTypeId=',
  endpointRentVehicleSearchVehicles: 'https://localhost:44367/api/Vehicle/SearchVehicles?vehicleTypeId=',
  endpointRentVehicleChangeAvailability: 'https://localhost:44367/api/Vehicle/ChangeAvailability',
  
  endpointRentvehicleLoadImageService: 'https://localhost:44367/api/Service/LoadImage?imageId=',
  endpointRentVehicleLoadImageVehicle: 'https://localhost:44367/api/Vehicle/LoadImage?imageId=',
  endpointRentVehicleLoadImageBranchOffice: 'https://localhost:44367/api/BranchOffice/LoadImage?imageId=',

  endpointBookingPostFeedback: 'https://localhost:44383/api/UserFeedback/PostUserFeedback',
  endpointBookingFeedbacksForService: 'https://localhost:44383/api/UserFeedback/GetUserFeedbacks?serviceId=',
  endpointBookingHasUserPostedFeedback: 'https://localhost:44383/api/UserFeedback/CanUserPostFeedback?serviceId=',
  endpointBookingCreateReservation: 'https://localhost:44383/api/Reservation/PostReservation',
};
