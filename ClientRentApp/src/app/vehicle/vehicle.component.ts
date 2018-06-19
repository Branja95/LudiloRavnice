import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { VehicleType } from '../models/vehicle-type.model';

import { VehicleService } from '../services/vehicle.service';
import { BranchOffice } from '../models/branch-office.model';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.css'],
  providers: [VehicleService]
})
export class VehicleComponent implements OnInit {
 
  public vehicleTypes: string[]
  public url = ''
  constructor(private vehicleService: VehicleService) { }

  ngOnInit() {
    this.getlanguages()
  }

  onSubmit(form: NgForm, branchOffice: BranchOffice) {
    this.vehicleService.postMethodCreateVehicle(branchOffice)
    .subscribe(
      data => {
        alert(data);
      },
      error => {
        alert(error.error.Message);
      })
  }

  getlanguages() { 
    this.vehicleService.getMethodVehicleTypes().subscribe(res => { this.vehicleTypes = res as string[] });  
  }

  onSelectFile(event) {
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); // read file as data url

      reader.onload = (event) => { // called once readAsDataURL is completed
         //this.url = event.target.result;
      }
    }
  }

}
