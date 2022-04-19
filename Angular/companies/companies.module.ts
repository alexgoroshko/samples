import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { CompaniesRoutingModule } from './companies-routing.module';
import { CompaniesComponent } from './companies.component';
import { UiModule } from '../../shared/ui/ui.module';
import { TranslateModule } from '@ngx-translate/core';
import { CreateEditCompanyComponent } from './create-edit-company/create-edit-company.component';
import { SitesModule } from '../sites/sites.module';
import { DirectivesModule } from '../../directives/directives.module';
import { ContractsModule } from '../contracts/contracts.module';

@NgModule({
  declarations: [CompaniesComponent, CreateEditCompanyComponent],
  imports: [CommonModule, CompaniesRoutingModule, SharedModule, UiModule, TranslateModule, SitesModule, DirectivesModule, ContractsModule]
})
export class CompaniesModule {}
