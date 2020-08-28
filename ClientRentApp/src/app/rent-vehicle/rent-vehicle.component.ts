import { Component, OnInit } from '@angular/core';
import { RentVehicle } from '../models/rent-vehicle.model';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-branch-office',
  templateUrl: './rent-vehicle.component.html',
  styleUrls: ['./rent-vehicle.component.css'],
  providers: [RentVehicleService]
})
export class RentVehicleComponent implements OnInit {

  loadImageService = environment.endpointRentvehicleLoadImageService;
  services = Array<RentVehicle>();
  
  constructor(private rentVehicleService: RentVehicleService) { }

  ngOnInit() {
    this.getServices();
  }

  isManagerOrAdmin(service){

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

  deleteService(serviceId){
    this.rentVehicleService.deleteService(serviceId).subscribe(
      res => { 
      console.log(res);
    }, error => {
      console.log(error.error.Message);
    });
    
  }
  
  getServices() { 
    this.rentVehicleService.getMethodServices()
    .subscribe(
      res => { 
          this.services = res as Array<RentVehicle>;
      }, error => {
        console.log(error);
      });
  }
}
