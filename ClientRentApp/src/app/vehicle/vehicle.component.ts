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


  vehicles = Array<Vehicle>()

  constructor(private vehicleService: VehicleService) { }

  ngOnInit() {
    this.getVehicles()
  }

  parseImages(imageId){
    let images = imageId.split(";_;");
    console.log(imageId);
    return images;
  }

  getVehicles() { 
    this.vehicleService.getMethodVehicles()
    .subscribe(
      res => { 
          this.vehicles = res as Array<Vehicle>;
          console.log(this.vehicles);
      }, error => {
        alert(error);
      }); 
  }

}
