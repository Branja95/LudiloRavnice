import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BranchOffice } from '../models/branch-office.model';
import { BranchOfficeService } from '../services/branch-office.service';
import { MapInfo } from '../models/map-info.model';

@Component({
  selector: 'app-add-branch-office',
  templateUrl: './add-branch-office.component.html',
  styleUrls: ['./add-branch-office.component.css'],
  providers: [BranchOfficeService]
})
export class AddBranchOfficeComponent implements OnInit {

  mapType = "add";
  ServiceId: string = "-1";
  url: string = '';

  file: File = null;
  mapInfoCooridnates: MapInfo;
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private branchOfficeService: BranchOfficeService) {
    activatedRoute.params.subscribe(params => {this.ServiceId = params["ServiceId"]});
  }

  ngOnInit() {
  }

  handleFileInput(event) {
    this.file = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); 
      reader.onload = (event) => { 
        this.url = reader.result as string;
      }
    }
  }

  receiveNewCoordinates($event) {
    this.mapInfoCooridnates = $event;
  }

  onSubmit(form: NgForm, branchOffice: BranchOffice) {

    branchOffice.serviceId = this.ServiceId;
    branchOffice.Latitude = this.mapInfoCooridnates.centerLat;
    branchOffice.Longitude = this.mapInfoCooridnates.centerLong;
   
    this.branchOfficeService.postMethodCreateBranchOffice(this.ServiceId, branchOffice, this.file)
    .subscribe(
      res => {
        this.router.navigateByUrl('/BranchOffice/' + this.ServiceId)
      }, error => {
        console.log(error);
      });;

    form.reset();
    this.url = '';
    this.file = null;
  }
  
}
