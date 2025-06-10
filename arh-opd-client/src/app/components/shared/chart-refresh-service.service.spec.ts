import { TestBed } from '@angular/core/testing';

import { ChartRefreshServiceService } from './chart-refresh-service.service';

describe('ChartRefreshServiceService', () => {
  let service: ChartRefreshServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChartRefreshServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
