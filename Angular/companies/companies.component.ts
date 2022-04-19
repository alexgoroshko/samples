import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { EMPTY, Observable, of } from 'rxjs';
import { finalize, map, switchMap, takeUntil } from 'rxjs/operators';
import { TablePageDataModel } from '../../common/models/table-page-data.model';
import { ConfirmDialogService } from '../../common/services/confirm-dialog.service';
import { SnackbarService } from '../../common/services/snackbar.service';
import { ExportService } from '../shared/export-button/export.service';
import { CreateEditUserDialogComponent } from '../users/create-edit-user-dialog/create-edit-user-dialog.component';
import { UserModel } from '../users/user.service';
import { CompaniesTableColumnNames } from './companies-table-column-names.enum';
import { CompaniesService, CompanyModel } from './companies.service';
import { COUNTRIES_LIST } from '../sites/create-edit-site-page/countries-list';
import { DataTableStateDirective } from '../../directives/data-table-state.directive';

@Component({
  selector: 'app-companies',
  templateUrl: './companies.component.html',
  styleUrls: ['./companies.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CompaniesComponent extends DataTableStateDirective<CompanyModel> implements OnInit {
  public companiesTableColumnNames = CompaniesTableColumnNames;

  public displayedColumns: CompaniesTableColumnNames[] = [
    CompaniesTableColumnNames.companyName,
    CompaniesTableColumnNames.vat,
    CompaniesTableColumnNames.parentCompanyName,
    CompaniesTableColumnNames.contactPersons,
    CompaniesTableColumnNames.siteCountry,
    CompaniesTableColumnNames.siteCity,
    CompaniesTableColumnNames.siteAddress,
    CompaniesTableColumnNames.sitePhone,
    CompaniesTableColumnNames.actions
  ];

  constructor(
    private companiesService: CompaniesService,
    private dialog: MatDialog,
    private confirmDialogService: ConfirmDialogService,
    public snackbarService: SnackbarService,
    public exportService: ExportService,
    public cdr: ChangeDetectorRef,
    private translateService: TranslateService,
    public activatedRoute: ActivatedRoute,
    public router: Router
  ) {
    super(activatedRoute, router, snackbarService, cdr, exportService);
  }

  public ngOnInit() {
    this.snackbarService.showDelayedSuccessSnackBar();
  }

  public openUserDialog(data: UserModel, companyName: string): void {
    data.companyName = companyName;
    this.dialog.open(CreateEditUserDialogComponent, {
      maxWidth: 600,
      data: { ...data, disableMode: true }
    });
  }

  public deleteCompany(company: CompanyModel): void {
    this.confirmDialogService
      .openConfirmationDialog({
        yesActionColor: 'warn',
        text: 'Confirmation.DeleteCompanyText',
        yesAction: 'Confirmation.DeleteCompanyActionYes',
        title: this.translateService.instant('Confirmation.DeleteCompany', { value: company.companyName }),
        noAction: 'Confirmation.DeleteActionNo'
      })
      .pipe(
        switchMap((res) => {
          if (res) {
            this.loading = true;
            this.cdr.markForCheck();

            return this.companiesService.deleteCompany(company.companyId).pipe(switchMap(() => this._fetchData()));
          } else {
            return EMPTY;
          }
        }),
        takeUntil(this.destroy$),
        finalize(() => {
          this.loading = false;
          this.cdr.markForCheck();
        })
      )
      .subscribe({
        next: () => {
          this.snackbarService.showSuccessSnackbar();

          this.cdr.markForCheck();
        },
        error: () => {
          this.snackbarService.showErrorSnackbar();

          this.cdr.markForCheck();
        }
      });
  }

  public export(csv: boolean) {
    this.baseExport(
      csv,
      map((res) => {
        return res.items.map((i) => {
          if (i.contactPersons?.length) {
            (i.contactPersons as unknown) = i.contactPersons.map((p) => p.displayName).join('; ');
          }

          return i;
        });
      })
    );
  }

  public getCountryName(countryCode: string) {
    return COUNTRIES_LIST[countryCode];
  }

  protected _fetchData(isCsv = false): Observable<TablePageDataModel<CompanyModel>> {
    if (!this.paginator || !this.filter) return of({ items: [], count: 0 });

    return this.baseFetchData(isCsv, this.companiesService);
  }
}
