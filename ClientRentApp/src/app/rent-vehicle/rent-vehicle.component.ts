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

  public url = ''
  constructor(private rentVehicleService: RentVehicleService) { }

  ngOnInit() {
  }

  onSubmit(form: NgForm, rentVehicle: RentVehicle) {
    console.log(rentVehicle);
    this.rentVehicleService.postMethodCreateRentVehicleService(rentVehicle)
    .subscribe(
      data => {
        alert(data);
      },
      error => {
        alert(error.error.Message);
      })
  }
}