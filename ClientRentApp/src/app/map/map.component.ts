import { Component, OnInit, Output, Input, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute} from '@angular/router';
import { BranchOfficeService } from '../services/branch-office.service';
import { MapInfo } from '../models/map-info.model';
import { BranchOffice } from '../models/branch-office.model';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css'],
  styles: ['agm-map {height: 500px; width: 700px;}']
})

export class MapComponent implements OnInit {
 
  @Input() serviceId: string;
  @Input() branchOfficeId: string;
  @Input() mapType: string;
 
  @Output() messageEvent = new EventEmitter<string>();
  @Output() newCoordinates = new EventEmitter<MapInfo>();

  centerLatitude =  45.254796;
  centerLongitude =  19.844581;

  mapInfo: MapInfo;
  branchOffice: any;
  branchOffices: BranchOffice[];

  ngOnInit() {
    if(this.mapType == "add")
    {
      this.mapInfo = new MapInfo(this.centerLatitude, this.centerLongitude, "", "" , "" , "");
    }
    else if(this.mapType == "edit")
    {
      this.getBranchOffice();
    }
    else 
    {
      this.getBranchOffices();
    }
  }

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private branchOfficeService: BranchOfficeService){ }

  isEdit():boolean{
    if(this.mapType == "edit" || this.mapType == "add")
    {
      return true;
    }
    else 
    {
      return false;
    }
  }

  getBranchOffice(){
    this.branchOfficeService.getBranchOffice(this.branchOfficeId).subscribe(
      res => {
        this.branchOffice = res as BranchOffice;
        this.mapInfo = new MapInfo(this.branchOffice.Latitude, this.branchOffice.Longitude, "", "" , "" , "");
      }, error =>{
        console.log(error);
      })
  }

  getBranchOffices(){
    this.branchOfficeService.getBranchOffices(this.serviceId).subscribe(
      res => {
        this.branchOffices = res;
        this.mapInfo = new MapInfo(this.centerLatitude, this.centerLongitude, "", "" , "" , "");
      }, error =>{
        console.log(error);
      })
  }

  previous;
  
  markerClicked(branchOfficeId: string, infowindow){
    this.messageEvent.emit(branchOfficeId);

    if (this.previous) {
      this.previous.close();
    }
    this.previous = infowindow;
  }

  placeMarker($event){
    this.mapInfo.centerLat = $event.coords.lat;
    this.mapInfo.centerLong = $event.coords.lng;

    this.newCoordinates.emit(this.mapInfo)
  }

}