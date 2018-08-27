import { Component, OnInit } from '@angular/core';

import { Router, ActivatedRoute, Params } from '@angular/router';

import { NgForm } from '@angular/forms';

import { VehicleType } from '../models/vehicle-type.model';

import { VehicleService } from '../services/vehicle.service';
import { BranchOffice } from '../models/branch-office.model';
import { Vehicle } from '../models/vehicle.model';
import { element } from 'protractor';

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
          console.log(this.vehicles);
      }, error => {
        alert(error);
      }); 
  }

  parseImages(imageId){
    
    return imageId.split(";_;");
  }

  onDelete(id){
    this.vehicleService.deleteVehicle(id)
    .subscribe(
      data => {
        alert(data);
      },
      error => {
        alert(error);
      })
  }

  getVehicleTypeName(vehicleTypeId){
    this.vehicleService.getVehicleType(vehicleTypeId)
    .subscribe(
      data => {
        this.vehicleType = data as string
        console.log(this.vehicleType);
      }
    )
  }
}
