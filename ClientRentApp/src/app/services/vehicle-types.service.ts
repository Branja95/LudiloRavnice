import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment'
import { Observable } from 'rxjs/Observable';

@Injectable({
  providedIn: 'root'
})
export class VehicleTypesService {

  constructor(private httpClient: HttpClient) { }

  getMethodVehicleType(vehicleTypeId): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleGetVehicleType + vehicleTypeId);
  }

  getMethodVehicleTypes(): Observable<any>{
    return this.httpClient.get(environment.endpointRentVehicleGetVehicleTypes);
  }

  postMethodVehicleTypes(vehicleTypeName): Observable<any>{
    return this.httpClient.post(environment.endpointRentVehicleTypeCreateVehicleType, vehicleTypeName);
  }

  putMethodVehicleTypes(vehicleType): Observable<any>{
    return this.httpClient.put(environment.endpointRentVehicleTypeEditVehicleType, vehicleType);
  }

}
