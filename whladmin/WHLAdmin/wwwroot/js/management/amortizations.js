$(function () {

  $('.whl-action-add-amortization').on('click', function (e) {
    e.preventDefault();
    $.get(amortizationAddUrl)
      .done(function (response) {
        $('#whl-title-amortization-action').html('Add Amortization');
        $('#hidAmortizationAction').val('ADD');
        $('#amortizationEditorModal').find('div.modal-body').html(response);
        $('#amortizationEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-amortization').on('click', function (e) {
    e.preventDefault();
    const rate = $(e.currentTarget).data('rate');
    $.get(amortizationEditUrl + '?rate=' + rate)
      .done(function (response) {
        $('#whl-title-amortization-action').html('Edit Amortization');
        $('#hidAmortizationAction').val('EDIT');
        $('#amortizationEditorModal').find('div.modal-body').html(response);
        $('#amortizationEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-amortization').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    const a = $('#hidAmortizationAction').val();
    const rate = $('#Rate').val();
    const url = a === 'EDIT' ? amortizationEditUrl : amortizationAddUrl;
    const data = {
      Rate: $('#Rate').val(),
      RateInterestOnly: $('#RateInterestOnly').val(),
      Rate10Year: $('#Rate10Year').val(),
      Rate15Year: $('#Rate15Year').val(),
      Rate20Year: $('#Rate20Year').val(),
      Rate25Year: $('#Rate25Year').val(),
      Rate30Year: $('#Rate30Year').val(),
      Rate40Year: $('#Rate40Year').val(),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#amortizationEditorModal').find('div.modal-body').html('');
        $('#amortizationEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Amortization ' + rate + ' saved, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save amortization information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-amortization').on('click', function (e) {
    e.preventDefault();
    const rate = $(e.currentTarget).data('rate');
    if (confirm('Are you sure you want to delete amortization - ' + rate + '?')) {
      $.post(amortizationDeleteUrl + '?rate=' + rate)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Amortization ' + rate + ' deleted, refreshing page.');
        });
    }
  });

});