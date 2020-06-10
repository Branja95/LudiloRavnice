import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { VehicleType } from '../models/vehicle-type.model';
import { VehicleService } from '../services/vehicle.service';
import { Vehicle } from '../models/vehicle.model';

@Component({
  selector: 'app-service-vehicles',
  templateUrl: './service-vehicles.component.html',
  styleUrls: ['./service-vehicles.component.css']
})
export class ServiceVehiclesComponent implements OnInit {

  vehicleTypes = Array<VehicleType>();
  vehicles = Array<Vehicle>()
  vehicleType: string;

  ServiceId : string = "-1";

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private vehicleService: VehicleService) {
    activatedRoute.params
    .subscribe(params => {
      this.ServiceId = params["ServiceId"];
    });
  }

  ngOnInit() {
    this.getVehicles()   
  }

  getVehicles() { 
    this.vehicleService.getServiceVehicles(this.ServiceId)
    .subscribe(
      res => { 
          this.vehicles = res as Array<Vehicle>;
      }, error => {
        alert(error.error.Message);
      }); 
  }

  parseImages(imageId){
    return imageId.split(";_;");
  }

  onDelete(id){
    this.vehicleService.deleteVehicle(id)
    .subscribe(
      res => {
        alert(res);
      },
      error => {
        alert(error.error.Message);
      })
  }

  onChange(id){
    this.vehicleService.changeAvailability(id)
    .subscribe(
      res => {
        this.vehicles = res as Array<Vehicle>
      },
      error => {
        console.log(error);
      })
  }

  getVehicleTypeName(vehicleTypeId){
    this.vehicleService.getVehicleType(vehicleTypeId).subscribe(
      res => {
        this.vehicleType = res as string;
      });
  }

  isManagerOrAdmin(){

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

  isLogged() : boolean
  {
    if(!localStorage.jwt)
    {
      return false;
    }
    
    return true;
  }
}
