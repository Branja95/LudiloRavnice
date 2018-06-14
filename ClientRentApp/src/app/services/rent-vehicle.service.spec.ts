import { TestBed, inject } from '@angular/core/testing';

import { RentVehicleService } from './rent-vehicle.service';

describe('RentVehicleService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RentVehicleService]
    });
  });

  it('should be created', inject([RentVehicleService], (service: RentVehicleService) => {
    expect(service).toBeTruthy();
  }));
});
