import { TestBed, inject } from '@angular/core/testing';

import { BranchOfficeService } from './branch-office.service';

describe('BranchOfficeService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BranchOfficeService]
    });
  });

  it('should be created', inject([BranchOfficeService], (service: BranchOfficeService) => {
    expect(service).toBeTruthy();
  }));
});
