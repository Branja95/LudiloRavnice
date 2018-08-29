import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule, JsonpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { HttpClientXsrfModule } from '@angular/common/http';
import { RouterModule, Routes } from '@angular/router';
import { NgForm  } from '@angular/forms';

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
    path: "Vehicles/:ServiceId",
    component: ServiceVehiclesComponent
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
    path: "VehicleTypes",
    component: VehicleTypesComponent
  },
  {
    path: "EditVehicleType/:VehicleTypeId",
    component: EditVehicleTypesComponent
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
    path: "ReserveAVehicle/:VehicleId",
    component: ReserveAVehicleComponent
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
  {
    path: "Map/:BranchOfficeId",
    component: MapComponent
  },
  {
    path: "Comment/:ServiceId",
    component: CommentComponent
  },
  {
    path: "Rating/:ServiceId",
    component: RatingComponent
  },
  {
    path: "AddComment/:ServiceId",
    component: AddCommentComponent
  },
  {
    path: "AddRating/:ServiceId",
    component: AddRatingComponent
  },
  {
    path: "EditComment/:commentId",
    component: EditCommentComponent
  },
  {
    path: "EditRating/:ratingId",
    component: EditRatingComponent
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
    EditRatingComponent
    VehicleTypesComponent,
    EditVehicleTypesComponent
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
    NotificationService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
