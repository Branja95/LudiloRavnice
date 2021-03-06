import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { VehicleTypesService } from '../services/vehicle-types.service';
import { VehicleType } from '../models/vehicle-type.model'

@Component({
  selector: 'app-vehicle-types',
  templateUrl: './vehicle-types.component.html',
  styleUrls: ['./vehicle-types.component.css']
})
export class VehicleTypesComponent implements OnInit {

  vehicleTypes: Array<VehicleType>;

  constructor(private vehicleTypesService: VehicleTypesService, private router: Router) { }

  ngOnInit() {
    this.vehicleTypesService.getMethodVehicleTypes()
    .subscribe(
      data => {
        this.vehicleTypes = data;
      },
      error => {
        alert(error.error.Message);
      });
  }

  onAdd(VehicleType){
    this.vehicleTypesService.postMethodVehicleTypes(VehicleType).subscribe(
      res => {
        alert(res);
        this.router.navigateByUrl("/VehicleTypes");
      },
      error => {
        alert(error);
      });
  }

}
