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
import { ClockComponent } from './clock/clock.component';
import { ApproveAccountComponent } from './approve-account/approve-account.component';

const Routes = [
  {
    path: "Login",
    component: LoginComponent
  },
  {
    path: "ApproveAccount",
    component: ApproveAccountComponent
  },
  {
    path: "Registration",
    component: RegistrationComponent
  },
  {
    path: "BranchOffice",
    component: BranchOfficeComponent
  },
  {
    path: "Vehicle",
    component: VehicleComponent
  },
  {
    path: "RentVehicle",
    component: RentVehicleComponent
  }
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
    ClockComponent,
    ApproveAccountComponent,
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
