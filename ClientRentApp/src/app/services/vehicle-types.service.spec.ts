import { TestBed, inject } from '@angular/core/testing';

import { VehicleTypesService } from './vehicle-types.service';

describe('VehicleTypesService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [VehicleTypesService]
    });
  });

  it('should be created', inject([VehicleTypesService], (service: VehicleTypesService) => {
    expect(service).toBeTruthy();
  }));
});
