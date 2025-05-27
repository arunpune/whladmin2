$(function () {

  $('.whl-action-add-unit').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingUnitAddUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-unit-action').html('Add Unit');
        $('#hidUnitAction').val('ADD');
        $('#unitEditorModal').find('div.modal-body').html(response);
        $('#unitEditorModal').modal('show');
        $('#lblUnitSaveErrorMessage').html('');
        $('#divUnitSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-unit').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const unitId = $(e.currentTarget).data('unitid');
    $.get(listingUnitEditUrl + '?unitId=' + unitId)
      .done(function (response) {
        $('#whl-title-unit-action').html('Edit Unit');
        $('#hidUnitAction').val('EDIT');
        $('#unitEditorModal').find('div.modal-body').html(response);
        $('#unitEditorModal').modal('show');
        $('#lblUnitSaveErrorMessage').html('');
        $('#divUnitSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-unit').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblUnitSaveErrorMessage').html('');
    $('#divUnitSaveErrorMessage').hide();
    const a = $('#hidUnitAction').val();
    const url = a === 'EDIT' ? listingUnitEditUrl : listingUnitAddUrl;
    const data = {
      UnitId: $('#UnitId').val(),
      ListingId: $('#UnitListingId').val(),
      UnitTypeCd: $('#UnitTypeCd').val(),
      BedroomCnt: $('#BedroomCnt').val(),
      BathroomCnt: $('#BathroomCnt').val(),
      BathroomCntPart: $('#BathroomCntPart').val(),
      AreaMedianIncomePct: $('#AreaMedianIncomePct').val(),
      UnitsAvailableCnt: $('#UnitsAvailableCnt').val(),
      MonthlyRentAmt: $('#MonthlyRentAmt').val(),
      AssetLimitAmt: $('#AssetLimitAmt').val(),
      EstimatedPriceAmt: $('#EstimatedPriceAmt').val(),
      SubsidyAmt: $('#SubsidyAmt').val(),
      MonthlyTaxesAmt: $('#MonthlyTaxesAmt').val(),
      MonthlyMaintenanceAmt: $('#MonthlyMaintenanceAmt').val(),
      MonthlyInsuranceAmt: $('#MonthlyInsuranceAmt').val(),
      SquareFootage: $('#SquareFootage').val(),
      MinHouseholdIncomeAmt1: $('#MinHouseholdIncomeAmt1').val(),
      MinHouseholdIncomeAmt2: $('#MinHouseholdIncomeAmt2').val(),
      MinHouseholdIncomeAmt3: $('#MinHouseholdIncomeAmt3').val(),
      MinHouseholdIncomeAmt4: $('#MinHouseholdIncomeAmt4').val(),
      MinHouseholdIncomeAmt5: $('#MinHouseholdIncomeAmt5').val(),
      MinHouseholdIncomeAmt6: $('#MinHouseholdIncomeAmt6').val(),
      MinHouseholdIncomeAmt7: $('#MinHouseholdIncomeAmt7').val(),
      MaxHouseholdIncomeAmt1: $('#MaxHouseholdIncomeAmt1').val(),
      MaxHouseholdIncomeAmt2: $('#MaxHouseholdIncomeAmt2').val(),
      MaxHouseholdIncomeAmt3: $('#MaxHouseholdIncomeAmt3').val(),
      MaxHouseholdIncomeAmt4: $('#MaxHouseholdIncomeAmt4').val(),
      MaxHouseholdIncomeAmt5: $('#MaxHouseholdIncomeAmt5').val(),
      MaxHouseholdIncomeAmt6: $('#MaxHouseholdIncomeAmt6').val(),
      MaxHouseholdIncomeAmt7: $('#MaxHouseholdIncomeAmt7').val(),
      Active: $('#UnitActive').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#unitEditorModal').find('div.modal-body').html('');
        $('#unitEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Units for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save unit';
        $('#lblUnitSaveErrorMessage').html(e.responseJSON.message);
        $('#divUnitSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-unit').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const unitId = $(e.currentTarget).data('unitid');
    if (confirm('Are you sure you want to delete listing unit - ' + unitId + '?')) {
      $.post(listingUnitDeleteUrl + '?unitId=' + unitId)
        .done(function (response) {
          const listingId = $('#PageListingId').val();
          fnShowToast('SUCCESS', 'Unit ' + unitId + ' for Listing #' + listingId + ' deleted, refreshing page.');
        });
    }
  });

});