$(function () {

  $('.whl-action-add-listing').on('click', function (e) {
    e.preventDefault();
    $.get(listingAddUrl)
      .done(function (response) {
        $('#whl-title-listing-action').html('Add Listing');
        $('#hidListingAction').val('ADD');
        $('#listingEditorModal').find('div.modal-body').html('').html(response);
        $('#listingEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-listing').on('click', function (e) {
    e.preventDefault();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-listing-action').html('Edit Listing');
        $('#hidListingAction').val('EDIT');
        $('#listingEditorModal').find('div.modal-body').html('').html(response);
        $('#listingEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-listing').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidListingAction').val();
    const url = a === 'EDIT' ? listingEditUrl : listingAddUrl;
    const data = {
      ListingId: $('#ListingId').val(),
      ListingTypeCd: $('#ListingTypeCd').val(),
      ResaleInd: $('#ResaleInd').prop('checked'),
      ListingAgeTypeCd: $('#ListingAgeTypeCd').val(),
      Name: $('#Name').val(),
      Description: $('#Description').val(),
      WebsiteUrl: $('#WebsiteUrl').val(),
      StreetLine1: $('#StreetLine1').val(),
      StreetLine2: $('#StreetLine2').val(),
      StreetLine3: $('#StreetLine3').val(),
      City: $('#City').val(),
      StateCd: $('#StateCd').val(),
      ZipCode: $('#ZipCode').val(),
      County: $('#County').val(),
      EsriX: $('#EsriX').val(),
      EsriY: $('#EsriY').val(),
      Municipality: $('#Municipality').val(),
      MunicipalityUrl: $('#MunicipalityUrl').val(),
      SchoolDistrict: $('#SchoolDistrict').val(),
      SchoolDistrictUrl: $('#SchoolDistrictUrl').val(),
      MapUrl: $('#MapUrl').val(),
      RentIncludesText: $('#RentIncludesText').val(),
      CompletedOrInitialOccupancyYear: $('#CompletedOrInitialOccupancyYear').val(),
      TermOfAffordability: $('#TermOfAffordability').val(),
      StatusCd: $('#StatusCd').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#listingEditorModal').find('div.modal-body').html('');
        $('#listingEditorModal').modal('hide');

        if (a === 'EDIT') {
          const listingId = $('#PageListingId').val();
          fnShowToast('SUCCESS', 'Listing #' + listingId + ' updated, refreshing page.');
        } else {
          const listingId = response.listingId;
          fnShowToast('SUCCESS',
            'Listing #' + listingId + ' created successfully. Redirecting to details page.',
            listingDetailsUrl + '?listingId=' + listingId);
        }
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save listing information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-listing').on('click', function (e) {
    e.preventDefault();
    const listingId = $(e.currentTarget).data('listingid');
    if (confirm('Are you sure you want to delete listing - ' + listingId + '?')) {
      $.post(listingDeleteUrl + '?listingId=' + listingId)
        .done(function (response) {
          window.location.reload();
        });
    }
  });

  $('.whl-action-cancel-listing').on('click', function (e) {
    $('#listingEditorModal').find('div.modal-body').html('');
    $('#listingEditorModal').modal('hide');
  });

  $('.whl-action-gotopage').on('click', function (e) {
    $('.whl-formfield-pageno').val($(this).data('pageno'));
    $('.whl-action-filter-listings').trigger('click');
  });

});