import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildMessageRenderer } from '../child-message-renderer/child-message-renderer.component';

describe('ChildMessageRendererComponent', () => {
  let component: ChildMessageRenderer;
  let fixture: ComponentFixture<ChildMessageRenderer>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChildMessageRenderer ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChildMessageRenderer);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
