import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { RentVehicle } from '../models/rent-vehicle.model';

import { RentVehicleService } from '../services/rent-vehicle.service';

@Component({
  selector: 'app-branch-office',
  templateUrl: './rent-vehicle.component.html',
  styleUrls: ['./rent-vehicle.component.css'],
  providers: [RentVehicleService]
})
export class RentVehicleComponent implements OnInit {

  url: string = '';
  file: File = null;

  constructor(private rentVehicleService: RentVehicleService) { }

  ngOnInit() {
  }

  handleFileInput(event) {
    this.file = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
    var reader = new FileReader();

    reader.readAsDataURL(event.target.files[0]); 

    reader.onload = (event) => { 
      this.url = reader.result;
    }

    }
  }

  onSubmit(form: NgForm, rentVehicle: RentVehicle) {
    
    this.rentVehicleService.postMethodCreateRentVehicleService(rentVehicle, this.file)
    .subscribe(
      data => {
        alert(data);
      }, error => {
        alert(error.error.Message);
      });;
    
    form.reset();
    this.url = '';
    this.file = null;
  }
}
