import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-approve-service-admin',
  templateUrl: './approve-service-admin.component.html',
  styleUrls: ['./approve-service-admin.component.css'],
  providers: [RentVehicleService]
})
export class ApproveServiceAdminComponent implements OnInit {

  private services : any;
  serviceLoadImage = environment.endpointRentvehicleLoadImageService;

  constructor(private rentVehicleService: RentVehicleService, private router: Router) { }

  ngOnInit() {
    this.rentVehicleService.getMethodServicesForApproves()
    .subscribe(
      res => {
        this.services = res;
      },
      error => {
        console.log(error);
      })
  }

  approveService(serviceId : number)
  {
    this.rentVehicleService.postMethodApproveService(serviceId)
    .subscribe(
      res => {
        this.router.navigate(['/RentVehicle']);
      },
      error => {
        console.log(error);
      })
  }

  rejectService(serviceId : number)
  {
    this.rentVehicleService.postMethodRejectService(serviceId)
    .subscribe(
      res => {
        this.router.navigate(['/RentVehicle']);
      },
      error => {
        console.log(error);
      })
  }
}
