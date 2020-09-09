import { Component, OnInit } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Vehicle } from '../models/vehicle.model';
import { VehicleType } from '../models/vehicle-type.model';
import { VehicleService } from '../services/vehicle.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-edit-vehicle',
  templateUrl: './edit-vehicle.component.html',
  styleUrls: ['./edit-vehicle.component.css']
})
export class EditVehicleComponent implements OnInit {

  VehicleId: string = "-1";
  VehicleAvailable: string = "";
  VehicleYearMade: string = "";
  Vehicle: Vehicle;
  
  serviceLoadImage = environment.endpointRentvehicleLoadImageService;
  vehicleLoadImage = environment.endpointRentVehicleLoadImageVehicle;
  urls: Array<string> = new Array<string>();
  uploadedFiles: FileList = null;
  vehicleTypes = Array<VehicleType>()
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private vehicleService: VehicleService) { 
    activatedRoute.params.subscribe(params => {this.VehicleId = params["VehicleId"]});
  }

  ngOnInit() {
    this.getVehicleTypes();
    this.vehicleService.getVehicle(this.VehicleId).subscribe(
      data => {
        this.Vehicle = data as Vehicle;
        this.getAvailable(data.IsAvailable);
        this.getYearMade(data.YearMade);
      },error => {
        console.log(error);
      });
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
    this.vehicleService.editVehicle(this.VehicleId, vehicle, this.uploadedFiles)
    .subscribe(
      data => {
        this.Vehicle = data as Vehicle;
        this.urls = [];
      }, error => {
        console.log(error);
      });
  }

  getVehicleTypes() { 
    this.vehicleService.getVehicleTypes()
    .subscribe(
      res => { 
        this.vehicleTypes = res as Array<VehicleType> 
    });  
  }

  parseImages(imageId){
    return imageId.split(";_;");
  }

  getAvailable(isAvailable : Boolean) {
    if(isAvailable)
    {
      this.VehicleAvailable = "Available";
    }
    else
    {
      this.VehicleAvailable = "Not Available";
    }
  }

  getYearMade(yearMade : string){
    let splited = yearMade.split("T");
    this.VehicleYearMade = splited[0];
  }

}
