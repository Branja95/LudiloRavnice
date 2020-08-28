import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute} from '@angular/router';
import { BranchOffice } from '../models/branch-office.model';
import { BranchOfficeService } from '../services/branch-office.service';
import { MapInfo } from '../models/map-info.model';
import { Location } from '@angular/common';

import { environment } from '../../environments/environment';
import { env } from 'process';

@Component({
  selector: 'app-edit-branch-office',
  templateUrl: './edit-branch-office.component.html',
  styleUrls: ['./edit-branch-office.component.css'],
  providers: [BranchOfficeService]
})

export class EditBranchOfficeComponent implements OnInit  {
  
  mapType = "edit";
  serviceId : string = "-1";
  branchOfficeId: string = "-1";
  selecetdFileUrl: string = '';
  
  branchOfficeLoadImage = environment.endpointRentVehicleLoadImageBranchOffice;

  selectedFile: File = null;
  mapInfoCooridnates: MapInfo;
  branchOffice: BranchOffice;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private location: Location, private branchOfficeService: BranchOfficeService) {
    activatedRoute.params
    .subscribe(params => {
      this.serviceId = params["ServiceId"];
      this.branchOfficeId = params["BranchOfficeId"];
    });
  }

  ngOnInit() {
    this.branchOfficeService.getBranchOffice(this.branchOfficeId).subscribe(
      res => {
        this.branchOffice = res as BranchOffice;
      },error => {
        alert(error.error.Message);
    });
  }

  handleFileInput(event) {
    this.selectedFile = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); 
      reader.onload = (event) => { 
        this.selecetdFileUrl = reader.result as string;
      }
    }
  }

  receiveNewCoordinates($event) {
    this.mapInfoCooridnates = $event;
  }

  onSubmit(form: NgForm) {
    this.branchOffice.latitude = this.mapInfoCooridnates.centerLat;
    this.branchOffice.longitude = this.mapInfoCooridnates.centerLong;

    this.branchOfficeService.editBranchOffice(this.serviceId, this.branchOfficeId, this.branchOffice, this.selectedFile)
    .subscribe(
      res => {
        console.log('1', res);
      }, error => {
        console.log('2', error);
      });;
    
    this.location.back();

    form.reset();
    this.selecetdFileUrl = '';
    this.selectedFile = null;
  }

}
