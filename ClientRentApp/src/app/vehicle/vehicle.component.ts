import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { VehicleType } from '../models/vehicle-type.model';

import { VehicleService } from '../services/vehicle.service';
import { BranchOffice } from '../models/branch-office.model';
import { Vehicle } from '../models/vehicle.model';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.css'],
  providers: [VehicleService]
})
export class VehicleComponent implements OnInit {

  urls: Array<string> = new Array<string>();
  uploadedFiles: FileList = null;
  
  vehicleTypes = Array<VehicleType>()
  vehicles = Array<Vehicle>()

  constructor(private vehicleService: VehicleService) { }

  ngOnInit() {
    this.getVehicleTypes()
    this.getVehicles()
  }

  handleFileInput(files: FileList) {
    this.uploadedFiles = files;
    
    let fileList = Array.from(files);
    
    if (fileList) {
      for (let file of fileList) {
        let reader = new FileReader();
        reader.onload = (e: any) => {
          this.urls.push(e.target.result);
        }
        reader.readAsDataURL(file);
      }
    }
  }

  onSubmit(form: NgForm, vehicle: Vehicle) {
    
    this.vehicleService.postMethodCreateVehicle(vehicle, this.uploadedFiles)
    .subscribe(
      data => {
        alert(data);
      }, error => {
        alert(error.error.Message);
      });;;

    form.resetForm();
    this.urls = new Array<string>();
    this.getVehicleTypes();
  }
  
  getVehicleTypes() { 
    this.vehicleService.getMethodVehicleTypes().subscribe(res => { this.vehicleTypes = res as Array<VehicleType> });  
  }

  getVehicles() { 
    this.vehicleService.getMethodVehicles()
    .subscribe(
      res => { 
          this.vehicles = res as Array<Vehicle>;
      }, error => {
        alert(error);
      }); 
  }

}
