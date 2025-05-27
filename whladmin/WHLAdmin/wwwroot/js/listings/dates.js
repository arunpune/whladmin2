$(function () {

  $('.whl-action-edit-dates').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingDatesEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-dates-action').html('Edit Dates/Times');
        $('#hidDatesAction').val('EDIT');
        $('#datesEditorModal').find('div.modal-body').html(response);
        $('#datesEditorModal').modal('show');
        $('#lblDatesSaveErrorMessage').html('');
        $('#divDatesSaveErrorMessage').hide();
      });
  });
  $('.whl-action-save-dates').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblDatesSaveErrorMessage').html('');
    $('#divDatesSaveErrorMessage').hide();

    const a = $('#hidDatesAction').val();
    const url = listingDatesEditUrl;
    const data = {
      ListingId: $('#DatesListingId').val(),
      ListingStartDate: $('#ListingStartDate').val(),
      ListingEndDate: $('#ListingEndDate').val(),
      ApplicationStartDate: $('#ApplicationStartDate').val(),
      ApplicationStartTime: $('#ApplicationStartTime').val(),
      ApplicationEndDate: $('#ApplicationEndDate').val(),
      ApplicationEndTime: $('#ApplicationEndTime').val(),
      LotteryEligible: $('#LotteryEligible').prop('checked'),
      LotteryDate: $('#LotteryDate').val(),
      LotteryTime: $('#LotteryTime').val(),
      WaitlistEligible: $('#WaitlistEligible').prop('checked'),
      WaitlistStartDate: $('#WaitlistStartDate').val(),
      WaitlistStartTime: $('#WaitlistStartTime').val(),
      WaitlistEndDate: $('#WaitlistEndDate').val(),
      WaitlistEndTime: $('#WaitlistEndTime').val()
    };

    $.post(url, data)
      .done(function (response) {
        $('#datesEditorModal').find('div.modal-body').html('');
        $('#datesEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Dates for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save dates';
        $('#lblDatesSaveErrorMessage').html(e.responseJSON.message);
        $('#divDatesSaveErrorMessage').show();
      });
  });

});