import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { VehicleType } from '../models/vehicle-type.model';
import { VehicleService } from '../services/vehicle.service';
import { PagerService } from '../services/pager.service';
import { Vehicle } from '../models/vehicle.model';
import { SearchVehicle } from '../models/searchVehicle.model';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-vehicle',
  templateUrl: './vehicle.component.html',
  styleUrls: ['./vehicle.component.css'],
  providers: [VehicleService]
})
export class VehicleComponent implements OnInit {

  pager: any = {};
  pagedVehicles: Array<Vehicle>;
  pageSize = 2;
  
  isSearch: boolean = false;

  VehicleId: string = "-1";
  vehicleType: string;

  searchVehicle: SearchVehicle;
  search: SearchVehicle;

  totalVehicles;
  vehicleTypeId;
  searchPrice;

  vehicleTypesSearch = Array<VehicleType>();
  vehicleTypes = Array<VehicleType>();
  vehicles = Array<Vehicle>()
  
  vehicleLoadImage = environment.endpointRentVehicleLoadImageVehicle;
  constructor(private vehicleService: VehicleService, private pagerService: PagerService) { }

  ngOnInit() {
    this.getVehicles()
    this.getTotalVehiclesCount();
    this.getVehicleTypes();
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

  getSearchNumberOfVehicles(searchVehicle: SearchVehicle){
    this.vehicleService.searchNumberOfVehicles(searchVehicle.VehicleTypeId, searchVehicle.PriceFrom, searchVehicle.PriceTo, searchVehicle.Manufactor, searchVehicle.Model).subscribe(
      res => {
        this.totalVehicles = res as number;
        this.pager.totalItems = this.totalVehicles;
      },
      error => {
        console.log(error);
      });
  }

  getVehicles() { 
    this.vehicleService.getVehicles()
    .subscribe(
      res => { 
          this.vehicles = res as Array<Vehicle>;
          this.setShowPage(1);
      }, error => {
        console.log(error);
      }); 
  }

  getSearchVehicles(searchVehicle: SearchVehicle){
    this.vehicleService.searchVehicles(searchVehicle.VehicleTypeId, searchVehicle.PriceFrom, searchVehicle.PriceTo, searchVehicle.Manufactor, searchVehicle.Model).subscribe(
      res => {
        this.vehicles = res as Array<Vehicle>;
        this.setShowPage(1);
      },
      error => {
        console.log(error);
      });
  }

  setShowPage(page: number) {
    this.pager = this.pagerService.getPager(this.vehicles.length, page);
    this.getPagedVehciles(this.pager.currentPage, this.search);
  }

  getPagedVehciles(pageIndex, searchVehicle: SearchVehicle){
    if(!this.isSearch){
      this.vehicleService.getPagedVehicles(pageIndex, this.pageSize)
      .subscribe(
        res => {
          this.pagedVehicles = res as Array<Vehicle>;
        },
        error=> {
          console.log(error);
        });
    }else{
      this.vehicleService.getSearchPagedVehicles(pageIndex, this.pageSize, searchVehicle.VehicleTypeId, searchVehicle.PriceFrom, searchVehicle.PriceTo, searchVehicle.Manufactor, searchVehicle.Model)
      .subscribe(
        res => {
          this.pagedVehicles = res as Array<Vehicle>;
        },
        error=> {
          console.log(error);
        });
    }
    
  }
  
  getSearchVehciles(pageIndex, searchVehicle: SearchVehicle){
    
  }

  getVehicleTypes(){
    this.vehicleService.getVehicleTypes()
    .subscribe(
      res => { 
          this.vehicleTypesSearch = res as Array<VehicleType>;
      }, error => {
        console.log(error);
      }); 
  }

  getVehicleTypeName(vehicleTypeId){
    this.vehicleService.getVehicleType(vehicleTypeId)
    .subscribe(
      res => {
        this.vehicleType = res as string
      },
      error => {
        console.log(error);
      });
  }

  parseImages(imageId){
      return imageId.split(";_;");
  }

  onDelete(id){
    this.vehicleService.deleteVehicle(id)
    .subscribe(
      data => {
      },
      error => {
      });
  }

  onSubmit(form: NgForm, searchVehicle: SearchVehicle){
    if(searchVehicle.VehicleTypeId == undefined || searchVehicle.VehicleTypeId == null)
    {
      searchVehicle.VehicleTypeId = "-1";
    }
    if(searchVehicle.PriceFrom == "" || searchVehicle.PriceFrom == null)
    {
      searchVehicle.PriceFrom = "-1";
    }
    if(searchVehicle.PriceTo == "" || searchVehicle.PriceTo == null)
    {
      searchVehicle.PriceTo = "-1";
    }
    if(searchVehicle.Manufactor == "" || searchVehicle.Manufactor == null)
    {
      searchVehicle.Manufactor = "-1";
    }
    if(searchVehicle.Model == "" || searchVehicle.Model == null)
    {
      searchVehicle.Model = "-1";
    }

    this.getSearchVehicles(searchVehicle);
    this.getSearchNumberOfVehicles(searchVehicle);

    this.search = searchVehicle;
    this.isSearch = true;

    form.reset();
  }
  
  isManagerOrAdmin(){
    if(!localStorage.role)
    {
      return false;
    }
    else
    {
      if(localStorage.role == "Manager" || localStorage.role == "Administrator")
      {
        return true;
      }
      return false;
    }
  }

  isLogged() : boolean{
    if(!localStorage.jwt)
    {
      return false;
    }
    
    return true;
  }
 
}
