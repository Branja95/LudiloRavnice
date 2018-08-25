import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientXsrfModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { NgForm  } from '@angular/forms';

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { TokenInterceptor } from './interceptors/token.interceptor';

import { NotificationService } from './services/notification.service';

import { AppComponent } from './app.component';
import { RegistrationComponent } from './registration/registration.component';
import { LoginComponent } from './login/login.component';
import { NavBarComponent } from './nav-bar/nav-bar.component';
import { BranchOfficeComponent } from './branch-office/branch-office.component';
import { VehicleComponent } from './vehicle/vehicle.component';
import { RentVehicleComponent } from './rent-vehicle/rent-vehicle.component';
import { ApproveAccountComponent } from './approve-account/approve-account.component';
import { AddBranchOfficeComponent } from './add-branch-office/add-branch-office.component';
import { AddVehicleComponent } from './add-vehicle/add-vehicle.component';
import { AddRentVehicleComponent } from './add-rent-vehicle/add-rent-vehicle.component';
import { EditBranchOfficeComponent } from './edit-branch-office/edit-branch-office.component';
import { ApproveAccountAdminComponent } from './approve-account-admin/approve-account-admin.component';
import { UserAccountComponent } from './user-account/user-account.component';
import { EditRentVehicleComponent } from './edit-rent-vehicle/edit-rent-vehicle.component';
import { EditVehicleComponent } from './edit-vehicle/edit-vehicle.component';
import { ApproveServiceAdminComponent } from './approve-service-admin/approve-service-admin.component';
import { ViewRentVehicleComponent } from './view-rent-vehicle/view-rent-vehicle.component';

const Routes = [
  {
    path: "Login",
    component: LoginComponent
  },
  {
    path: "UserAccount",
    component: UserAccountComponent
  },
  {
    path: "ApproveAccount",
    component: ApproveAccountComponent
  },
  {
    path: "ApproveAccountAdmin",
    component: ApproveAccountAdminComponent
  },
  {
    path: "ApproveServiceAdmin",
    component: ApproveServiceAdminComponent
  },
  {
    path: "Registration",
    component: RegistrationComponent
  },
  {
    path: "BranchOffice/:ServiceId",
    component: BranchOfficeComponent
  },
  {
    path: "Vehicle",
    component: VehicleComponent
  },
  {
    path: "RentVehicle",
    component: RentVehicleComponent
  },
  {
    path: "AddBranchOffice/:ServiceId",
    component: AddBranchOfficeComponent
  }, 
  {
    path: "EditBranchOffice/:ServiceId/:BranchOfficeId",
    component: EditBranchOfficeComponent
  },
  {
    path: "AddVehicle/:ServiceId",
    component: AddVehicleComponent
  }, 
  {
    path: "EditVehicle/:VehicleId",
    component: EditVehicleComponent
  },
  {
    path: "AddService",
    component: AddRentVehicleComponent
  },
  {
    path: "EditService/:ServiceId",
    component: EditRentVehicleComponent
  },
  {
    path: "ViewService/:ServiceId",
    component: ViewRentVehicleComponent
  },
]

@NgModule({
  declarations: [
    AppComponent,
    RegistrationComponent,
    LoginComponent,
    NavBarComponent,
    BranchOfficeComponent,
    VehicleComponent,
    RentVehicleComponent,
    ApproveAccountComponent,
    AddBranchOfficeComponent,
    EditBranchOfficeComponent,
    AddVehicleComponent,
    EditVehicleComponent,
    AddRentVehicleComponent,
    EditRentVehicleComponent,
    ApproveAccountAdminComponent,
    UserAccountComponent,
    ApproveServiceAdminComponent,
    ViewRentVehicleComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(Routes),
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule,
    HttpClientXsrfModule,
    JsonpModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    },
    NotificationService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
