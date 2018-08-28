import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { VehicleType } from '../models/vehicle-type.model';

import { VehicleService } from '../services/vehicle.service';
import { PagerService } from '../services/pager.service';
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

  // array of all items to be paged
  private allItems: any[];
  
  // pager object
  pager: any = {};

  // paged items
  pagedVehicles: Array<Vehicle>;

  pageSize = 2;
  totalVehicles;

  vehicleTypes = Array<VehicleType>();
  vehicles = Array<Vehicle>()
  vehicleType: string;

  constructor(private vehicleService: VehicleService, private pagerService: PagerService) { }

  ngOnInit() {
    this.getVehicles()
    this.getTotalVehiclesCount();
  }

  getVehicles() { 
    this.vehicleService.getVehicles()
    .subscribe(
      res => { 
          this.vehicles = res as Array<Vehicle>;
          this.setPage(1);
          console.log(this.vehicles);
      }, error => {
        alert(error);
      }); 
  }

  getTotalVehiclesCount(){
    this.vehicleService.getNumberOfVehicles()
    .subscribe(
      res => {
        this.totalVehicles = res as number;
        this.pager.totalItems = this.totalVehicles;
      },
      error => {
        console.log(error);
      }
    );
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
      },
      error => {
        console.log(error);
      }
    )
  }

  getPagedVehciles(pageIndex){
    this.vehicleService.getPagedVehicles(pageIndex, this.pageSize)
    .subscribe(
      res => {
        this.pagedVehicles = res as Array<Vehicle>;
        console.log(this.pagedVehicles);
      },
      error=> {
        console.log(error);
      }
    )
  }

  setPage(page: number) {
    this.pager = this.pagerService.getPager(this.vehicles.length, page);

    this.getPagedVehciles(this.pager.currentPage);
  }

  isManagerOrAdmin(){

    if(!localStorage.role)
    {
      return false;
    }
    else
    {
      if(localStorage.role == "Manager" || localStorage.role == "Admin")
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
