import { Component, OnInit } from '@angular/core';
import { VehicleType } from '../models/vehicle-type.model';
import { Router, ActivatedRoute } from '@angular/router';
import { VehicleTypesService } from '../services/vehicle-types.service';

@Component({
  selector: 'app-edit-vehicle-types',
  templateUrl: './edit-vehicle-types.component.html',
  styleUrls: ['./edit-vehicle-types.component.css']
})
export class EditVehicleTypesComponent implements OnInit {

  vehicleType: VehicleType;
  VehicleTypeId: string;

  constructor(private vehicleTypesService: VehicleTypesService, private activatedRoute: ActivatedRoute, private router: Router) { 
    activatedRoute.params.subscribe(params =>{this.VehicleTypeId = params["VehicleTypeId"]});
    this.ngOnInit();
  }

  ngOnInit() {
    this.vehicleTypesService.getMethodVehicleType(this.VehicleTypeId)
    .subscribe(
      data => {
        this.vehicleType = data;
      },
      error => {
        console.log(error);
      });
  }

  onEdit(vehicleType: VehicleType){
    vehicleType.id = this.VehicleTypeId;

    this.vehicleTypesService.putMethodVehicleTypes(vehicleType).subscribe(
      res => {
        this.router.navigateByUrl("/VehicleTypes");
      },
      error => {
        console.log(error);
    });
  }

}
