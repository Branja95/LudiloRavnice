import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { VehicleType } from '../models/vehicle-type.model';

import { VehicleService } from '../services/vehicle.service';
import { BranchOffice } from '../models/branch-office.model';
import { Vehicle } from '../models/vehicle.model';
import { element } from 'protractor';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.css'],
  providers: [VehicleService]
})
export class VehicleComponent implements OnInit {

  vehicleTypes = Array<VehicleType>();
  vehicles = Array<Vehicle>()
  vehicleType: string;

  constructor(private vehicleService: VehicleService) { }

  ngOnInit() {
    this.getVehicles()   
  }

  getVehicles() { 
    this.vehicleService.getVehicles()
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
