import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'sc-about',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './about.component.html'
})
export class AboutComponent {}
