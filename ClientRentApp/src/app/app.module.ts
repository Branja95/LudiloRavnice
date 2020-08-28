import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientXsrfModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { NgForm  } from '@angular/forms';
import { ToastModule } from '@ng-uikit-pro-standard';

import { AgmCoreModule } from '@agm/core';

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
import { ServiceVehiclesComponent } from './service-vehicles/service-vehicles.component';
import { MapComponent } from './map/map.component';
import { RatingComponent } from './rating/rating.component';
import { CommentComponent } from './comment/comment.component';
import { AddRatingComponent } from './add-rating/add-rating.component';
import { AddCommentComponent } from './add-comment/add-comment.component';
import { ReserveAVehicleComponent } from './reserve-a-vehicle/reserve-a-vehicle.component';
import { VehicleTypesComponent } from './vehicle-types/vehicle-types.component';
import { EditVehicleTypesComponent } from './edit-vehicle-types/edit-vehicle-types.component';
import { EditCommentComponent } from './edit-comment/edit-comment.component';
import { EditRatingComponent } from './edit-rating/edit-rating.component';
import { ChangeRolesComponent } from './change-roles/change-roles.component';
import { BanManagersComponent } from './ban-managers/ban-managers.component';

import { AdminGuard } from './guard/admin.guard';
import { AuthGuard } from './guard/auth.guard';
import { AMGuard } from './guard/am.guard';

const Routes = [
  {
    path: "Login",
    component: LoginComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "UserAccount",
    component: UserAccountComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "ApproveAccount",
    component: ApproveAccountComponent,
    canActivate: [AuthGuard]

  },
  {
    path: "ApproveAccountAdmin",
    component: ApproveAccountAdminComponent,
    canActivate: [AdminGuard]
  },
  {
    path: "ApproveServiceAdmin",
    component: ApproveServiceAdminComponent,
    canActivate: [AdminGuard]
  },
  {
    path: "Registration",
    component: RegistrationComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "BranchOffice/:ServiceId",
    component: BranchOfficeComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "Vehicles/:ServiceId",
    component: ServiceVehiclesComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "BranchOffice",
    component: BranchOfficeComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "Vehicle",
    component: VehicleComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "VehicleTypes",
    component: VehicleTypesComponent,
    canActivate: [AdminGuard]
  },
  {
    path: "EditVehicleType/:VehicleTypeId",
    component: EditVehicleTypesComponent,
    canActivate: [AdminGuard]
  },
  {
    path: "RentVehicle",
    component: RentVehicleComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "AddBranchOffice/:ServiceId",
    component: AddBranchOfficeComponent,
    canActivate: [AMGuard]
  }, 
  {
    path: "EditBranchOffice/:ServiceId/:BranchOfficeId",
    component: EditBranchOfficeComponent,
    canActivate: [AMGuard]
  },
  {
    path: "AddVehicle/:ServiceId",
    component: AddVehicleComponent,
    canActivate: [AMGuard]
  }, 
  {
    path: "EditVehicle/:VehicleId",
    component: EditVehicleComponent,
    canActivate: [AMGuard]
  },
  {
    path: "ReserveAVehicle/:VehicleId",
    component: ReserveAVehicleComponent,
    canActivate: [AuthGuard] 
  },
  {
    path: "AddService",
    component: AddRentVehicleComponent,
    canActivate: [AMGuard]
  },
  {
    path: "EditService/:ServiceId",
    component: EditRentVehicleComponent,
    canActivate: [AMGuard]
  },
  {
    path: "ViewService/:ServiceId",
    component: ViewRentVehicleComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "Map/:BranchOfficeId",
    component: MapComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "Comment/:ServiceId",
    component: CommentComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "Rating/:ServiceId",
    component: RatingComponent,
    canActivate: ['CanAlwaysActivateGuard']
  },
  {
    path: "AddComment/:ServiceId",
    component: AddCommentComponent,
    canActivate: [AuthGuard] 
  },
  {
    path: "AddRating/:ServiceId",
    component: AddRatingComponent,
    canActivate: [AuthGuard] 
  },
  {
    path: "EditComment/:commentId",
    component: EditCommentComponent,
    canActivate: [AuthGuard] 
  },
  {
    path: "EditRating/:ratingId",
    component: EditRatingComponent,
    canActivate: [AuthGuard] 
  },
  {
    path: "ChangeRoles",
    component: ChangeRolesComponent,
    canActivate: [AdminGuard] 
  },
  {
    path: "BanManagers",
    component: BanManagersComponent,
    canActivate: [AdminGuard] 
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
    ViewRentVehicleComponent,
    ServiceVehiclesComponent,
    ViewRentVehicleComponent,
    MapComponent,
    RatingComponent,
    CommentComponent,
    AddRatingComponent,
    AddCommentComponent,
    ReserveAVehicleComponent,
    EditCommentComponent,
    EditRatingComponent,
    VehicleTypesComponent,
    EditVehicleTypesComponent,
    ChangeRolesComponent,
    BanManagersComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(Routes),
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule,
    HttpClientXsrfModule,
    JsonpModule,
    AgmCoreModule.forRoot({apiKey: 'AIzaSyDnihJyw_34z5S1KZXp90pfTGAqhFszNJk'})
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    },
    NotificationService,
    {
      provide: 'CanAlwaysActivateGuard',
      useValue: () => {
        return true;
      } 
    },
    AdminGuard,
    AuthGuard,
    AMGuard
  ],
  bootstrap: [AppComponent]
})

export class AppModule { }
