import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { Vehicle } from '../models/vehicle.model';

@Component({
  selector: 'app-view-rent-vehicle',
  templateUrl: './view-rent-vehicle.component.html',
  styleUrls: ['./view-rent-vehicle.component.css']

})
export class ViewRentVehicleComponent implements OnInit {

  ServiceId: string = "-1";
  rentVehicle: Vehicle;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) {
    activatedRoute.params.subscribe(params => {
      this.ServiceId = params["ServiceId"]
      console.log(this.ServiceId);
    });
  }

  ngOnInit() {
    this.rentVehicleService.getService(this.ServiceId).subscribe(
      res => {
        this.rentVehicle = res as Vehicle;
      },error => {
        alert(error);
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
