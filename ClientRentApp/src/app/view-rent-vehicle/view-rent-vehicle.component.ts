import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { Vehicle } from '../models/vehicle.model';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-view-rent-vehicle',
  templateUrl: './view-rent-vehicle.component.html',
  styleUrls: ['./view-rent-vehicle.component.css']

})
export class ViewRentVehicleComponent implements OnInit {

  ServiceId: string = "-1";
  rentVehicle :Vehicle;
  serviceLoadImage = environment.endpointRentvehicleLoadImageService;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) {
    activatedRoute.params.subscribe(params => {
      this.ServiceId = params["ServiceId"]
    });
  }

  ngOnInit() {
    this.rentVehicleService.getService(this.ServiceId).subscribe(
      res => {
        this.rentVehicle = res as Vehicle;
      },error => {
        console.log('error',error);
      });
  }

  isManagerOrAdmin(service){

    if(!localStorage.role)
    {
      return false;
    }
    else
    {
      if(localStorage.role == "Manager" || localStorage.role == "Administrator")
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
