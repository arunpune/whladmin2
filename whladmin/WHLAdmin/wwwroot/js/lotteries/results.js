$(function () {

  // Run Lottery
  $('.whl-action-run-lottery').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $('.whl-label-confirm-listing-id').html(listingId);
    $('.whl-label-confirm-listing-name').html($(e.currentTarget).data('listingname'));
    $('.whl-label-confirm-listing-address').html($(e.currentTarget).data('listingaddress'));
    $('.whl-action-confirm-run-lottery').data('listingid', listingId);
    $('#runLotteryModal').modal('show');
    $('.whl-action-cancel-run-lottery').show();
    $('.whl-action-confirm-run-lottery').show();
    $('.whl-label-confirm-run-lottery-inprogress').hide();
  });
  $('.whl-action-confirm-run-lottery').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    var listingId = $(e.currentTarget).data('listingid');
    $('.whl-action-cancel-run-lottery').hide();
    $('.whl-action-confirm-run-lottery').hide();
    $('.whl-label-confirm-run-lottery-inprogress').show();
    $.post(lotteryRunUrl + '?listingId=' + listingId + '&rerun=true')
      .done(function (response) {
        $('#runLotteryModal').modal('hide');
        const lotteryId = response.lotteryId;
        fnShowToast('SUCCESS', 'Lottery #' + lotteryId + ' run successfully, redirecting to results page.', lotteryResultsUrl + '?lotteryId=' + lotteryId);
      })
      .fail(function (error) {
        $('#runLotteryModal').modal('hide');
        var message = error.responseJSON && error.responseJSON.message && error.responseJSON.message.length > 0 ? error.responseJSON.message : 'Failed to re-run lottery for Listing #' + listingId + ' due to one or more errors';
        if (error.responseJSON && error.responseJSON.details && error.responseJSON.details.length > 0) {
          message += '<br />' + error.responseJSON.details;
        }
        fnShowToastForPage('FAILURE', message);
      });
  });

  $('.whl-action-download-results').on('click', function (e) {
    e.preventDefault();
    const lotteryId = $(e.currentTarget).data('lotteryid');
    const listingId = $(e.currentTarget).data('listingid');
    window.open(lotteryResultsUrl + '?lotteryId=' + lotteryId + '&listingId=' + listingId + '&download=Y', '_whladminDownload');
  });

  $('.whl-action-gotopage').on('click', function (e) {
    $('.whl-formfield-pageno').val($(this).data('pageno'));
    $('.whl-action-filter-applications').trigger('click');
  });

});