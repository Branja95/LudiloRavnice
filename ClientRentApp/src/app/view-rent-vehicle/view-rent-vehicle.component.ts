import { Component, OnInit } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';

import { RentVehicle } from '../models/rent-vehicle.model';
import { RentVehicleService } from '../services/rent-vehicle.service';

@Component({
  selector: 'app-view-rent-vehicle',
  templateUrl: './view-rent-vehicle.component.html',
  styleUrls: ['./view-rent-vehicle.component.css']

})
export class ViewRentVehicleComponent implements OnInit {

  ServiceId: string = "-1";
  rentVehicle: any;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) {
    activatedRoute.params.subscribe(params => {
      this.ServiceId = params["ServiceId"]
      console.log(this.ServiceId);
    });
  }

  ngOnInit() {
    this.rentVehicleService.getService(this.ServiceId).subscribe(
      res => {
        this.rentVehicle = res;
        console.log(this.rentVehicle);
      },error => {
        alert(error.error.Message);
      });
  }

  isManagerOrAdmin(service){

    if(!localStorage.role)
    {
      return false;
    }
    else
    {
      if(localStorage.role == "Manager" || localStorage.role == "Admin")
      {
        return true;
      }
      return false;
    }
  }

  deleteService(serviceId){
    this.rentVehicleService.deleteService(serviceId).subscribe(
      res => { 
      console.log(res);
    }, error => {
      alert(error);
    });
    
  }
}
