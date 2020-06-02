import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';

@Component({
  selector: 'app-approve-service-admin',
  templateUrl: './approve-service-admin.component.html',
  styleUrls: ['./approve-service-admin.component.css'],
  providers: [RentVehicleService]
})
export class ApproveServiceAdminComponent implements OnInit {

  private services : any;

  constructor(private rentVehicleService: RentVehicleService, private router: Router) { }

  ngOnInit() {
    this.rentVehicleService.getMethodServicesForApproves()
    .subscribe(
      res => {
        this.services = res;
        console.log(res);
      },
      error => {
        console.log(error);
      })
  }

  approveService(id : number)
  {
    this.rentVehicleService.postMethodApproveService(id)
    .subscribe(
      res => {
        console.log(res);
        this.router.navigate(['/RentVehicle']);
      },
      error => {
        console.log(error);
      })
  }

  rejectService(id : number)
  {
    this.rentVehicleService.postMethodRejectService(id)
    .subscribe(
      res => {
        console.log(res);
        this.router.navigate(['/RentVehicle']);
      },
      error => {
        console.log(error);
      })
  }
}
